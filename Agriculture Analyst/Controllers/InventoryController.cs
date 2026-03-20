using System.Security.Claims;
using Agriculture_Analyst.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class InventoryController : Controller
{
    private readonly IInventoryTransactionService _service;
    private readonly AgricultureAnalystDbContext _context;

    public InventoryController(
        IInventoryTransactionService service,
        AgricultureAnalystDbContext context)
    {
        _service = service;
        _context = context;
    }

    // ================== TRANG CHỦ LỊCH SỬ GIAO DỊCH ==================
    public IActionResult Index(int? type, int? invId, int? itemId, DateTime? fromDate, DateTime? toDate)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // 1. Lấy dữ liệu đã lọc
        var data = _service.GetUserTransactions(userId, type, invId, itemId, fromDate, toDate);

        // 2. Chuẩn bị dữ liệu cho Dropdown
        ViewBag.InventoryList = new SelectList(_context.Inventories.Where(x => x.UserId == userId), "InvId", "InvName", invId);
        ViewBag.ItemList = new SelectList(_context.Items, "ItemId", "ItemName", itemId);

        // 3. Giữ lại giá trị cũ để hiển thị trên Form
        ViewBag.Type = type;
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

        return View(data);
    }

    // ================== TẠO MỚI (NHẬP KHO) ==================
    public IActionResult Create()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // Chỉ load kho của user hiện tại
        ViewBag.InventoryList = new SelectList(
            _context.Inventories.Where(x => x.UserId == userId).ToList(),
            "InvId",
            "InvName"
        );

        ViewBag.ItemList = new SelectList(
            _context.Items.ToList(),
            "ItemId",
            "ItemName"
        );

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventoryTransaction trans, IFormFile? imageFile)
    {
        // Lấy ID người dùng đang đăng nhập
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        trans.UserId = userId;
        trans.Type = 1; // 1 = Nhập kho
        trans.NgayGiaoDich = DateTime.Now;
        trans.ThanhTien = trans.SoLuong * trans.DonGia;

        // ✅ ĐOẠN CODE UPLOAD ẢNH LÊN CLOUDINARY (Đã bọc kiểm tra null an toàn)
        if (imageFile != null && imageFile.Length > 0)
        {
            try
            {
                Account account = new Account(
                    "dyop5mqdp",           // Cloud Name
                    "818898689433435",     // API Key
                    "FUXXYjFM6mhjT3tGexPuoOSoEKc" // Secret key
                );
                Cloudinary cloudinary = new Cloudinary(account);

                using (var stream = imageFile.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, stream),
                        Folder = "Inventory_Imports"
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

                    // Kiểm tra xem Cloudinary có trả về lỗi không, nếu không mới gán URL
                    if (uploadResult.Error != null)
                    {
                        Console.WriteLine("❌ LỖI UPLOAD ẢNH CLOUDINARY: " + uploadResult.Error.Message);
                    }
                    else if (uploadResult.SecureUrl != null)
                    {
                        // Lưu link ảnh HTTPS vào Database an toàn
                        trans.ImageUrl = uploadResult.SecureUrl.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi nếu cấu hình sai account
                Console.WriteLine("❌ LỖI HỆ THỐNG CLOUDINARY: " + ex.Message);
            }
        }

        // Lưu giao dịch vào Database
        _context.InventoryTransactions.Add(trans);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // ================== XUẤT KHO (GET) ==================
    public IActionResult Export(int? filterInvId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // 1. Query lấy lô hàng (Lọc theo kho nếu người dùng đã chọn)
        var query = _context.InventoryTransactions
            .Include(t => t.Item)
            .Include(t => t.Inventory)
            .Where(t => t.UserId == userId && t.Type == 1);

        if (filterInvId.HasValue)
        {
            query = query.Where(t => t.InvId == filterInvId.Value);
        }

        // Kéo dữ liệu về bộ nhớ
        var rawBatches = query.OrderByDescending(t => t.NgayGiaoDich).ToList();

        // Khởi tạo Repo để tính số tồn
        var repo = new InventoryTransactionRepository(_context);

        // ✅ CHỈ LẤY NHỮNG LÔ HÀNG CÒN TỒN KHO (> 0) VÀ HIỂN THỊ SỐ TỒN
        var importBatches = rawBatches
            .Select(t => new {
                Batch = t,
                Remaining = repo.GetBatchRemainingQuantity(t.TransId)
            })
            .Where(x => x.Remaining > 0) // Bộ lọc cốt lõi: Chỉ lấy lô còn hàng
            .Select(x => new
            {
                TransId = x.Batch.TransId,
                DisplayText = filterInvId.HasValue
                    ? $"{x.Batch.Item.ItemName} - Nhập: {x.Batch.NgayGiaoDich:dd/MM/yyyy} - Tồn: {x.Remaining} - Giá: {x.Batch.DonGia:N0}"
                    : $"{x.Batch.Item.ItemName} ({x.Batch.Inventory.InvName}) - Nhập: {x.Batch.NgayGiaoDich:dd/MM/yyyy} - Tồn: {x.Remaining}"
            })
            .ToList();

        ViewBag.BatchList = new SelectList(importBatches, "TransId", "DisplayText");

        // 2. List danh sách kho để người dùng chọn lọc
        ViewBag.InventoryList = new SelectList(
            _context.Inventories.Where(x => x.UserId == userId).ToList(),
            "InvId", "InvName", filterInvId);

        // 3. Danh sách cây trồng (Chỉ lấy cây đang trồng)
        ViewBag.PlantList = new SelectList(_context.Plants.Where(u => u.UserId == userId && u.Status == "Đang trồng"), "PlantId", "PlantName");

        return View();
    }

    // ================== XUẤT KHO (POST) ==================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Export(InventoryTransaction model)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        ModelState.Remove("Inventory");
        ModelState.Remove("Item");
        ModelState.Remove("Plant");
        ModelState.Remove("User");

        // CHECK LOGIC: Kiểm tra người dùng có chọn Lô hàng không?
        if (model.RefTransId == null || model.RefTransId == 0)
        {
            ModelState.AddModelError("RefTransId", "Vui lòng chọn lô hàng cần xuất!");
        }
        else
        {
            var repo = new InventoryTransactionRepository(_context);
            int remaining = repo.GetBatchRemainingQuantity(model.RefTransId.Value);

            if (model.SoLuong > remaining)
            {
                ModelState.AddModelError("SoLuong", $"Lô này chỉ còn {remaining} (Bạn đang xuất {model.SoLuong})");
            }
        }

        // Nếu có lỗi (chưa chọn lô hoặc xuất quá số lượng) -> Trả về View báo lỗi
        if (!ModelState.IsValid)
        {
            var repo = new InventoryTransactionRepository(_context);
            var rawBatches = _context.InventoryTransactions
                .Include(t => t.Item)
                .Include(t => t.Inventory)
                .Where(t => t.UserId == userId && t.Type == 1)
                .OrderByDescending(t => t.NgayGiaoDich)
                .ToList();

            // ✅ LỌC LẠI LÔ CÒN TỒN NẾU FORM BỊ LỖI PHẢI LOAD LẠI
            var importBatches = rawBatches
                .Select(t => new {
                    Batch = t,
                    Remaining = repo.GetBatchRemainingQuantity(t.TransId)
                })
                .Where(x => x.Remaining > 0)
                .Select(x => new
                {
                    TransId = x.Batch.TransId,
                    DisplayText = $"{x.Batch.Item.ItemName} (Kho: {x.Batch.Inventory.InvName}) - Nhập: {x.Batch.NgayGiaoDich:dd/MM/yyyy} - Tồn: {x.Remaining} - Giá: {x.Batch.DonGia:N0}"
                })
                .ToList();

            ViewBag.BatchList = new SelectList(importBatches, "TransId", "DisplayText", model.RefTransId);
            ViewBag.PlantList = new SelectList(_context.Plants.Where(u => u.UserId == userId && u.Status == "Đang trồng"), "PlantId", "PlantName", model.PlantId);

            return View(model);
        }

        // Nếu mọi thứ OK -> Gán dữ liệu và Lưu
        try
        {
            model.UserId = userId;
            model.NgayGiaoDich = DateTime.Now;
            model.Type = 2; // Đánh dấu là Xuất kho (Export)
            model.ThanhTien = model.SoLuong * model.DonGia;

            // ✅ TỰ ĐỘNG LẤY ẢNH TỪ LÔ HÀNG GỐC GẮN VÀO LỆNH XUẤT
            var loHangGoc = _context.InventoryTransactions.FirstOrDefault(t => t.TransId == model.RefTransId);
            if (loHangGoc != null)
            {
                model.ImageUrl = loHangGoc.ImageUrl;
            }

            _service.Create(model);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            return View(model);
        }
    }


    // ================== API LẤY THÔNG TIN LÔ HÀNG QUA AJAX ==================
    [HttpGet]
    public IActionResult GetBatchDetails(int transId)
    {
        var repo = new InventoryTransactionRepository(_context);
        var remaining = repo.GetBatchRemainingQuantity(transId);

        var trans = _context.InventoryTransactions
            .Where(t => t.TransId == transId)
            .Select(t => new {
                t.ItemId,
                t.InvId,
                t.DonGia,
                t.LaiSuat,
                NgayNhap = t.NgayGiaoDich,
                RemainingQty = remaining
            })
            .FirstOrDefault();

        return Json(trans);
    }


    // ================== BÁO CÁO TỒN KHO ==================
    public IActionResult StockReport(int? invId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        ViewBag.InventoryList = new SelectList(
            _context.Inventories.Where(x => x.UserId == userId).ToList(),
            "InvId", "InvName", invId);

        var reportData = _service.GetCurrentStock(userId, invId);
        ViewBag.SelectedInvId = invId;

        return View(reportData);
    }
}