namespace Agriculture_Analyst.Models.ViewModel
{
    public class DiaryIndexViewModel
    {
        public Plant Plant { get; set; } = null!;
        public List<PlantTask> Tasks { get; set; } = new();
        public List<DiaryEntry> DiaryEntries { get; set; } = new();
        public DateTime SelectedDate { get; set; }
    }

}
