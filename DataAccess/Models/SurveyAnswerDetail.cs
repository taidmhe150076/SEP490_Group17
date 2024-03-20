using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SurveyAnswerDetail
{
    public int Id { get; set; }

    public int WorkShopSurveyQuestionId { get; set; }

    public int AnswerSurveyId { get; set; }

    public DateTime? DateTime { get; set; }

    public string? ParticipantsEmail { get; set; }

    public string? AnswerText { get; set; }

    public virtual AnswerSurvey AnswerSurvey { get; set; } = null!;

    public virtual WorkShopSurveyQuestion WorkShopSurveyQuestion { get; set; } = null!;
}
