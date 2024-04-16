using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class UrlForm
{
    public int Id { get; set; }

    public int WorkshopSurveyUrl { get; set; }

    public int WorkshopId { get; set; }

    public string? UrlForm1 { get; set; }

    public bool? IsPresenter { get; set; }

    public virtual Workshop Workshop { get; set; } = null!;

    public virtual WorkshopSurveyUrl WorkshopSurveyUrlNavigation { get; set; } = null!;
}
