using Agriculture_Analyst.Models;

public class ItemRepository : IItemRepository
{
    private readonly AgricultureAnalystDbContext _context;
    public ItemRepository(AgricultureAnalystDbContext context) { _context = context; }

    public IEnumerable<Item> GetAll() => _context.Items.ToList();

    public Item GetById(int id) => _context.Items.Find(id);

    public void Add(Item item)
    {
        _context.Items.Add(item);
        _context.SaveChanges();
    }

    public void Update(Item item)
    {
        _context.Items.Update(item);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var item = _context.Items.Find(id);
        if (item != null)
        {
            _context.Items.Remove(item);
            _context.SaveChanges();
        }
    }
}