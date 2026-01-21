using Agriculture_Analyst.Models;
using Microsoft.EntityFrameworkCore;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly AgricultureAnalystDbContext _context;

    public InventoryTransactionRepository(AgricultureAnalystDbContext context)
    {
        _context = context;
    }

    public void Add(InventoryTransaction entity)
    {
        _context.InventoryTransactions.Add(entity);
        _context.SaveChanges();
    }

    public IEnumerable<InventoryTransaction> GetByUser(int userId)
    {
        return _context.InventoryTransactions
            .Include(x => x.Item)
            .Include(x => x.Inventory)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.NgayGiaoDich)
            .ToList();
    }
}
