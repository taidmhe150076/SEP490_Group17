using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkshopSurveyUrl
{
    public int Id { get; set; }

    public int? WorkshopSeriesId { get; set; }

    public int? WorkshopId { get; set; }

    public string? Url { get; set; }

    public DateTime? AddedDate { get; set; }

    public string? SurveyName { get; set; }

    public int? AddedBy { get; set; }

    public string? FileByte { get; set; }

    public string? FileType { get; set; }

    public virtual ICollection<UrlForm> UrlForms { get; set; } = new List<UrlForm>();
}
