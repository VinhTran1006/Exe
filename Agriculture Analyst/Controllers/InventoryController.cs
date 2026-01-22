using System.Security.Claims;
using Agriculture_Analyst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

    public IActionResult Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(_service.GetUserTransactions(userId));
    }

    public IActionResult Create()
    {
        ViewBag.InventoryList = new SelectList(
            _context.Inventories.ToList(),
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
    public IActionResult Create(InventoryTransaction model)
    {
        ModelState.Remove("Inventory");
        ModelState.Remove("Item");
        ModelState.Remove("User");   // 👈 QUAN TRỌNG: Thiếu dòng này là lỗi
        ModelState.Remove("Plant");
        if (!ModelState.IsValid)
        {
            ViewBag.Items = _context.Items
    .Select(i => new SelectListItem
    {
        Value = i.ItemId.ToString(),
        Text = i.ItemName
    })
    .ToList();

            ViewBag.Inventories = _context.Inventories
                .Select(i => new SelectListItem
                {
                    Value = i.InvId.ToString(),
                    Text = i.InvName
                })
                .ToList();

            return View(model);
        }

        model.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        model.NgayGiaoDich = DateTime.Now;

        _service.Create(model);

        return RedirectToAction(nameof(Index));
    }
    // File: Controllers/InventoryController.cs

    public IActionResult Export()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // 1. Lấy danh sách các giao dịch NHẬP kho (Type = 1)
        // Để hiển thị ra dropdown: "Tên vật tư - Ngày nhập - Giá nhập"
        var importBatches = _context.InventoryTransactions
            .Include(t => t.Item)
            .Include(t => t.Inventory)
            .Where(t => t.UserId == userId && t.Type == 1) // Chỉ lấy đơn Nhập
            .OrderByDescending(t => t.NgayGiaoDich)
            .Select(t => new
            {
                TransId = t.TransId,
                DisplayText = $"{t.Item.ItemName} (Kho: {t.Inventory.InvName}) - Nhập: {t.NgayGiaoDich:dd/MM/yyyy} - Giá: {t.DonGia:N0}"
            })
            .ToList();

        ViewBag.BatchList = new SelectList(importBatches, "TransId", "DisplayText");

        // Giữ nguyên list cây trồng
        ViewBag.PlantList = new SelectList(_context.Plants.Where(u => u.UserId == userId && u.Status == "Đang trồng"), "PlantId", "PlantName");

        return View();
    }

    [HttpGet]
    public IActionResult GetBatchDetails(int transId)
    {
        // ✅ SỬA LỖI: Khởi tạo Repository thủ công từ _context
        var repo = new InventoryTransactionRepository(_context);

        // Gọi hàm từ biến 'repo' vừa tạo (chứ không phải _repo)
        var remaining = repo.GetBatchRemainingQuantity(transId);

        var trans = _context.InventoryTransactions
            .Where(t => t.TransId == transId)
            .Select(t => new {
                t.ItemId,
                t.InvId,
                t.DonGia,
                t.LaiSuat,
                NgayNhap = t.NgayGiaoDich,
                RemainingQty = remaining // Trả về số lượng tồn tính được
            })
            .FirstOrDefault();

        return Json(trans);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Export(InventoryTransaction model)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        ModelState.Remove("Inventory");
        ModelState.Remove("Item");
        ModelState.Remove("Plant");
        ModelState.Remove("User");

        // 3. CHECK LOGIC: Kiểm tra người dùng có chọn Lô hàng không?
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

        // 5. Nếu có lỗi (chưa chọn lô hoặc xuất quá số lượng) -> Trả về View báo lỗi
        if (!ModelState.IsValid)
        {
            var importBatches = _context.InventoryTransactions
                .Include(t => t.Item)
                .Include(t => t.Inventory)
                .Where(t => t.UserId == userId && t.Type == 1)
                .OrderByDescending(t => t.NgayGiaoDich)
                .Select(t => new
                {
                    TransId = t.TransId,
                    DisplayText = $"{t.Item.ItemName} (Kho: {t.Inventory.InvName}) - Nhập: {t.NgayGiaoDich:dd/MM/yyyy} - Giá: {t.DonGia:N0}"
                })
                .ToList();

            ViewBag.BatchList = new SelectList(importBatches, "TransId", "DisplayText", model.RefTransId);
            ViewBag.PlantList = new SelectList(_context.Plants.Where(u => u.UserId == userId && u.Status == "Đang trồng"), "PlantId", "PlantName", model.PlantId);

            return View(model);
        }

        // 6. Nếu mọi thứ OK -> Gán dữ liệu và Lưu
        try
        {
            model.UserId = userId;
            model.NgayGiaoDich = DateTime.Now;
            model.Type = 2; // Đánh dấu là Xuất kho (Export)

            // RefTransId đã tự động bind từ Dropdown, nên hệ thống sẽ biết xuất từ lô nào

            _service.Create(model);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
            return View(model);
        }
    }

    public IActionResult StockReport()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var reportData = _service.GetCurrentStock(userId);

        return View(reportData);
    }
}
