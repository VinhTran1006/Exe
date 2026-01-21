namespace Agriculture_Analyst.Models
{
    public class InventoryTransaction
    {
        public int TransId { get; set; }

        public int UserId { get; set; }

        public int InvId { get; set; }      // ✅ FK RÕ RÀNG
        public int ItemId { get; set; }     // ✅ FK RÕ RÀNG

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public DateTime? NgayGiaoDich { get; set; }
        public decimal? LaiSuat { get; set; }

        // navigation
        public Inventory Inventory { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
