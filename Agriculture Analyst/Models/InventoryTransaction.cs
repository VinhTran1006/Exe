// File: Models/InventoryTransaction.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agriculture_Analyst.Models
{
    // Enum để phân loại giao dịch
    public enum TransactionType
    {
        Import = 1, // Nhập kho
        Export = 2  // Xuất dùng
    }

    public class InventoryTransaction
    {
        [Key]
        public int TransId { get; set; }

        public int UserId { get; set; }

        public int InvId { get; set; }

        public int ItemId { get; set; }

        public int? RefTransId { get; set; }

        // --- CÁC CỘT MỚI THÊM ---
        public int? PlantId { get; set; } // Xuất cho cây nào (có thể null)
        public int Type { get; set; } = (int)TransactionType.Import; // Loại giao dịch
        // -------------------------

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }

        // Cột này được tính toán trong Database, nhưng khai báo ở đây để hứng dữ liệu khi query
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal ThanhTien { get; set; }

        // ✅ ĐÂY LÀ DÒNG BỊ THIẾU, BẠN THÊM LẠI NHÉ:
        public decimal? LaiSuat { get; set; }

        public DateTime? NgayGiaoDich { get; set; }

        // --- Navigation Properties ---
        public Inventory Inventory { get; set; } = null!;
        public Item Item { get; set; } = null!;
        public User User { get; set; } = null!;

        // Link tới cây trồng (nếu có)
        public Plant? Plant { get; set; }
    }
}