using System;
using System.Collections.Generic;
namespace Agriculture_Analyst.Models
{
    public partial class PlantTask
    {
        public int TaskId { get; set; }

        public int PlantId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public string? Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime NextDate { get; set; }

        public bool IsRecurring { get; set; }

        public int? RepeatIntervalDays { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual Plant? Plant { get; set; }

        public virtual User? User { get; set; }
    }
}
