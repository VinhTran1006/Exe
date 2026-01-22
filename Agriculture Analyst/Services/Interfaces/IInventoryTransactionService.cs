using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.DTOs;

public interface IInventoryTransactionService
{
    void Create(InventoryTransaction trans);
    IEnumerable<InventoryTransaction> GetUserTransactions(int userId);
    IEnumerable<InventoryReportViewModel> GetCurrentStock(int userId);
}
