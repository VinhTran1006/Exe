namespace Agriculture_Analyst.Models
{
    public class Inventory
    {
        public int InvId { get; set; }
        public int UserId { get; set; }

        public string InvName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
            = new List<InventoryTransaction>();
    }
}
