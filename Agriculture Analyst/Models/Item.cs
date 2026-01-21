namespace Agriculture_Analyst.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Unit { get; set; }

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
            = new List<InventoryTransaction>();
    }
}
