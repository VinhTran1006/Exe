using System;
using System.Collections.Generic;

namespace Agriculture_Analyst.Models;

public partial class Plant
{
    public int PlantId { get; set; }

    public int UserId { get; set; }

    public string PlantName { get; set; } = null!;

    public string? PlantType { get; set; }

    public DateTime? StartDate { get; set; }

    public double? Area { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<DiaryEntry> DiaryEntries { get; set; } = new List<DiaryEntry>();

    public virtual ICollection<PlantImage> PlantImages { get; set; } = new List<PlantImage>();

    public virtual User? User { get; set; }
    public virtual ICollection<PlantTask> PlantTasks { get; set; } = new List<PlantTask>();
}
