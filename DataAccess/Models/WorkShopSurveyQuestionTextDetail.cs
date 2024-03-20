using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkShopSurveyQuestionTextDetail
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string? AnswerText { get; set; }

    public string? ParticipantsEmail { get; set; }

    public virtual WorkShopSurveyQuestion Question { get; set; } = null!;
}
