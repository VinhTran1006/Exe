namespace Agriculture_Analyst.Models.DTOs
{
    // Models/DTO/YearHeatmapDto.cs
    public class YearHeatmapDto
    {
        public int Year { get; set; }
        public List<HeatmapDayDto> Days { get; set; } = new();
    }

    public class HeatmapDayDto
    {
        public string Date { get; set; } = "";
        public int Count { get; set; }
    }
}
