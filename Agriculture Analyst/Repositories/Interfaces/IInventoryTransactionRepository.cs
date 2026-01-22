using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;

public interface IInventoryTransactionRepository
{
    void Add(InventoryTransaction entity);
    IEnumerable<InventoryTransaction> GetByUser(int userId);
    int GetCurrentStock(int invId, int itemId);
    IEnumerable<InventoryReportViewModel> GetInventoryReport(int userId);
    int GetBatchRemainingQuantity(int batchId);
}
