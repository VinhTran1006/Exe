using Agriculture_Analyst.Models;

public interface IItemService
{
    IEnumerable<Item> GetAll();
    Item GetById(int id);
    void Add(Item item);
    void Update(Item item);
    void Delete(int id);
}