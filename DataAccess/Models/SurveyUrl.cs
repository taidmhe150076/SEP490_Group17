using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SurveyUrl
{
    public int Id { get; set; }

    public int? WorkshopSeriesId { get; set; }

    public int? WorkshopId { get; set; }

    public string? SurveyUrl1 { get; set; }

    public virtual Workshop? Workshop { get; set; }

    public virtual WorkshopSeries? WorkshopSeries { get; set; }
}
