using Agriculture_Analyst.Models;

public interface IInventoryTransactionRepository
{
    void Add(InventoryTransaction entity);
    IEnumerable<InventoryTransaction> GetByUser(int userId);
}
