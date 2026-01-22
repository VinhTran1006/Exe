using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;
using Microsoft.EntityFrameworkCore;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly AgricultureAnalystDbContext _context;

    public InventoryTransactionRepository(AgricultureAnalystDbContext context)
    {
        _context = context;
    }

    public void Add(InventoryTransaction entity)
    {
        _context.InventoryTransactions.Add(entity);
        _context.SaveChanges();
    }

    public IEnumerable<InventoryTransaction> GetByUser(int userId)
    {
        return _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Inventory)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.NgayGiaoDich)
            .ToList();
    }
    public int GetCurrentStock(int invId, int itemId)
    {
        var transactions = _context.InventoryTransactions
            .Where(x => x.InvId == invId && x.ItemId == itemId)
            .Select(x => new { x.Type, x.SoLuong })
            .ToList();

        int imported = transactions.Where(x => x.Type == 1).Sum(x => x.SoLuong);
        int exported = transactions.Where(x => x.Type == 2).Sum(x => x.SoLuong);

        return imported - exported;
    }
    public IEnumerable<InventoryReportViewModel> GetInventoryReport(int userId)
    {
        var query = _context.InventoryTransactions
            .Where(t => t.UserId == userId)
            .GroupBy(t => t.Item) // Nhóm theo Item (Vật tư)
            .Select(g => new InventoryReportViewModel
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.ItemName,
                Unit = g.Key.Unit,

                // Tính tổng nhập (Type = 1)
                TotalImport = g.Where(x => x.Type == 1).Sum(x => x.SoLuong),

                // Tính tổng xuất (Type = 2)
                TotalExport = g.Where(x => x.Type == 2).Sum(x => x.SoLuong),

                // Tồn = Nhập - Xuất
                StockQuantity = g.Where(x => x.Type == 1).Sum(x => x.SoLuong)
                              - g.Where(x => x.Type == 2).Sum(x => x.SoLuong)
            });

        return query.ToList();
    }

    // Thêm vào Interface trước: 
    // int GetBatchRemainingQuantity(int batchId);

    public int GetBatchRemainingQuantity(int batchId)
    {
        // 1. Lấy thông tin lô nhập gốc
        var importTrans = _context.InventoryTransactions.FirstOrDefault(x => x.TransId == batchId);
        if (importTrans == null) return 0;

        // 2. Tính tổng số lượng đã xuất từ lô này (dựa vào RefTransId)
        var usedQuantity = _context.InventoryTransactions
            .Where(x => x.RefTransId == batchId && x.Type == 2) // Type 2 là xuất
            .Sum(x => x.SoLuong);

        // 3. Trả về số còn lại
        return importTrans.SoLuong - usedQuantity;
    }
}
