using Agriculture_Analyst.Models;

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
}
