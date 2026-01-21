using System;
using System.Collections.Generic;

namespace Agriculture_Analyst.Models;

public partial class PlantImage
{
    public int ImageId { get; set; }

    public int PlantId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime? UploadDate { get; set; }

    public virtual ICollection<Aianalysis> Aianalyses { get; set; } = new List<Aianalysis>();

    public virtual Plant Plant { get; set; } = null!;
}
