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

    public int? StatusId { get; set; }

    public string? KeyPresenter { get; set; }

    public int? Index { get; set; }

    public virtual User? Presenter { get; set; }

    public virtual StatusWorkShop? Status { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<WorkShopSurveyQuestion> WorkShopSurveyQuestions { get; set; } = new List<WorkShopSurveyQuestion>();

    public virtual ICollection<WorkshopQuestion> WorkshopQuestions { get; set; } = new List<WorkshopQuestion>();

    public virtual WorkshopSeries? WorkshopSeries { get; set; }
}
