using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;

public interface IInventoryTransactionService
{
    void Create(InventoryTransaction trans);
    IEnumerable<InventoryTransaction> GetUserTransactions(int userId, int? type = null, int? invId = null, int? itemId = null, DateTime? fromDate = null, DateTime? toDate = null);
    IEnumerable<InventoryReportViewModel> GetCurrentStock(int userId, int? invId);
}
