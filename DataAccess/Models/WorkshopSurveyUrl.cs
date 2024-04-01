using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkshopSurveyUrl
{
    public int Id { get; set; }

    public int? WorkshopSeriesId { get; set; }

    public int? WorkshopId { get; set; }

    public string? SurveyUrl { get; set; }

    public string? SurveyKey { get; set; }

    public virtual Workshop? Workshop { get; set; }

    public virtual WorkshopSeries? WorkshopSeries { get; set; }
}
