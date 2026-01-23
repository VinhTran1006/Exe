using Agriculture_Analyst.Models;

public class InventoryRepository : IInventoryRepository
{
    private readonly AgricultureAnalystDbContext _context;
    public InventoryRepository(AgricultureAnalystDbContext context) { _context = context; }

    public IEnumerable<Inventory> GetByUser(int userId)
        => _context.Inventories.Where(x => x.UserId == userId).ToList();

    public Inventory GetById(int id) => _context.Inventories.Find(id);

    public void Add(Inventory inventory)
    {
        _context.Inventories.Add(inventory);
        _context.SaveChanges();
    }

    public void Update(Inventory inventory)
    {
        _context.Inventories.Update(inventory);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var inv = _context.Inventories.Find(id);
        if (inv != null)
        {
            _context.Inventories.Remove(inv);
            _context.SaveChanges();
        }
    }
}