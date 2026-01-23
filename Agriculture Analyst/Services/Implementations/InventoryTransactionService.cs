using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;

public class InventoryTransactionService : IInventoryTransactionService
{
    private readonly IInventoryTransactionRepository _repo;

    public InventoryTransactionService(IInventoryTransactionRepository repo)
    {
        _repo = repo;
    }

    public void Create(InventoryTransaction trans)
    {
        _repo.Add(trans);
    }

    public IEnumerable<InventoryTransaction> GetUserTransactions(int userId)
    {
        return _repo.GetByUser(userId);
    }

    public void Export(InventoryTransaction trans)
    {
        // 1. Kiểm tra tồn kho trước khi xuất
        int currentStock = _repo.GetCurrentStock(trans.InvId, trans.ItemId);
        if (currentStock < trans.SoLuong)
        {
            throw new Exception($"Kho không đủ hàng! Tồn: {currentStock}, Muốn xuất: {trans.SoLuong}");
        }

        // 2. Gán loại là Export và Lưu
        trans.Type = (int)TransactionType.Export;
        // Giá xuất có thể là 0 hoặc tính theo bình quân gia quyền (ở đây tôi để user tự nhập hoặc mặc định 0)
        _repo.Add(trans);
    }
    public IEnumerable<InventoryReportViewModel> GetCurrentStock(int userId, int? invId)
    {
        return _repo.GetInventoryReport(userId, invId);
    }

}
