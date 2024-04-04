using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Survey
{
    public int Id { get; set; }

    public int? WorkshopSerieId { get; set; }

    public int? WorkshopId { get; set; }

    public string? Url { get; set; }

    public virtual Workshop? Workshop { get; set; }

    public virtual WorkshopSeries? WorkshopSerie { get; set; }
}
