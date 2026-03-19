using Agriculture_Analyst.Models;
using System.Collections.Generic;

namespace Agriculture_Analyst.Models.ViewModel
{
    public class PlantReportViewModel
    {
        public Plant Plant { get; set; } = null!;

        // Danh sách vật tư đã dùng cho cây này
        public List<InventoryTransaction> UsedMaterials { get; set; } = new();

        // Tổng tiền chi phí vật tư
        public decimal TotalMaterialCost { get; set; }

        // Nhật ký hoạt động
        public List<DiaryEntry> DiaryEntries { get; set; } = new();
    }
}