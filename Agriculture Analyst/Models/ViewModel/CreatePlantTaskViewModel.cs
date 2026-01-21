namespace Agriculture_Analyst.Models.ViewModel
{
    public class CreatePlantTaskViewModel
    {
        public int PlantId { get; set; }

        public string Title { get; set; } = null!;
        public string? Note { get; set; }

        public DateTime NextDate { get; set; }

        public bool IsRecurring { get; set; }
        public int? RepeatIntervalDays { get; set; }
    }

}
