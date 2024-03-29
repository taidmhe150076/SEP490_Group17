using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkShopSurveyQuestionType
{
    public int Id { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<WorkShopSurveyQuestion> WorkShopSurveyQuestions { get; set; } = new List<WorkShopSurveyQuestion>();
}
