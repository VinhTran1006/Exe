using Agriculture_Analyst.Models;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repo;
    public InventoryService(IInventoryRepository repo) { _repo = repo; }

    public IEnumerable<Inventory> GetByUser(int userId) => _repo.GetByUser(userId);
    public Inventory GetById(int id) => _repo.GetById(id);
    public void Add(Inventory inventory) => _repo.Add(inventory);
    public void Update(Inventory inventory) => _repo.Update(inventory);
    public void Delete(int id) => _repo.Delete(id);
}