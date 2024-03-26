using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Participant
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public int? WorkshopSeriesId { get; set; }

    public DateTime? TimeStamp { get; set; }

    public string? Major { get; set; }

    public string? FavoriteTopics { get; set; }

    public virtual WorkshopSeries? WorkshopSeries { get; set; }
}
