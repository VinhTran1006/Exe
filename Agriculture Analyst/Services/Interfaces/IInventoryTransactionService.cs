using Agriculture_Analyst.Models;

public interface IInventoryTransactionService
{
    void Create(InventoryTransaction trans);
    IEnumerable<InventoryTransaction> GetUserTransactions(int userId);
}
