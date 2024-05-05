using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SentimentAnswerResult
{
    public int Id { get; set; }

    public int? SurveyId { get; set; }

    public string? Question { get; set; }

    public string? SentimentAnswer { get; set; }

    public virtual WorkshopSurveyUrl? Survey { get; set; }
}
