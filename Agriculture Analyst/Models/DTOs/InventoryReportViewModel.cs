namespace Agriculture_Analyst.Models.DTOs
{
    public class InventoryReportViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int TotalImport { get; set; } // Tổng nhập
        public int TotalExport { get; set; } // Tổng xuất
        public int StockQuantity { get; set; } // Tồn kho thực tế
    }
}