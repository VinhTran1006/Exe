using Agriculture_Analyst.Models;

public interface IInventoryService
{
    IEnumerable<Inventory> GetByUser(int userId);
    Inventory GetById(int id);
    void Add(Inventory inventory);
    void Update(Inventory inventory);
    void Delete(int id);
}