using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SurveyQuestionType
{
    public int Id { get; set; }

    public string QuestionType { get; set; } = null!;
}
