using System;
using System.Collections.Generic;

namespace Agriculture_Analyst.Models;

public partial class Aianalysis
{
    public int AnalysisId { get; set; }

    public int ImageId { get; set; }

    public string? HealthStatus { get; set; }

    public string? DiseaseDetected { get; set; }

    public string? Suggestion { get; set; }

    public DateTime? AnalyzedAt { get; set; }

    public virtual PlantImage Image { get; set; } = null!;
}
