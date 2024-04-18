using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkshopSeries
{
    public int Id { get; set; }

    public string? WorkshopSeriesName { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
}
