using System;
using System.Collections.Generic;

namespace Agriculture_Analyst.Models;

public partial class DiaryEntry
{
    public int DiaryId { get; set; }

    public int PlantId { get; set; }

    public string? Activity { get; set; }

    public string? Description { get; set; }

    public string? Weather { get; set; }

    public DateTime? EntryDate { get; set; }

    public virtual Plant Plant { get; set; } = null!;
}
