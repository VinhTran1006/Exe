using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;
using Agriculture_Analyst.Models.ViewModel; // Nhớ using namespace chứa ViewModel

public interface IInventoryTransactionRepository
{
    void Add(InventoryTransaction entity);
    IEnumerable<InventoryTransaction> GetByUser(int userId);
    int GetCurrentStock(int invId, int itemId); // Hàm check tồn tổng
    int GetBatchRemainingQuantity(int batchId); // Hàm check tồn lô

    IEnumerable<InventoryReportViewModel> GetInventoryReport(int userId, int? invId);
}