using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SurveyQuestionDatum
{
    public int Id { get; set; }

    public int? SurveyId { get; set; }

    public string? Question { get; set; }

    public int QuestionTypeId { get; set; }
}
