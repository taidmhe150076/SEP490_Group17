using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Workshop
{
    public int Id { get; set; }

    public string? WorkshopName { get; set; }

    public DateTime? DatePresent { get; set; }

    public int? PresenterId { get; set; }

    public int? WorkshopSeriesId { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual User? Presenter { get; set; }

    public virtual StatusWorkShop Status { get; set; } = null!;

    public virtual ICollection<WorkShopSurveyQuestion> WorkShopSurveyQuestions { get; set; } = new List<WorkShopSurveyQuestion>();

    public virtual ICollection<WorkshopQuestion> WorkshopQuestions { get; set; } = new List<WorkshopQuestion>();

    public virtual WorkshopSeries? WorkshopSeries { get; set; }
}
