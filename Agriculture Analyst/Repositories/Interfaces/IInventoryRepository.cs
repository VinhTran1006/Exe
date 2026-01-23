using Agriculture_Analyst.Models;

public interface IInventoryRepository
{
    IEnumerable<Inventory> GetByUser(int userId);
    Inventory GetById(int id);
    void Add(Inventory inventory);
    void Update(Inventory inventory);
    void Delete(int id);
}