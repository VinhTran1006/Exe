using Agriculture_Analyst.Models;

public class ItemService : IItemService
{
    private readonly IItemRepository _repo;
    public ItemService(IItemRepository repo) { _repo = repo; }

    public IEnumerable<Item> GetAll() => _repo.GetAll();
    public Item GetById(int id) => _repo.GetById(id);
    public void Add(Item item) => _repo.Add(item);
    public void Update(Item item) => _repo.Update(item);
    public void Delete(int id) => _repo.Delete(id);
}