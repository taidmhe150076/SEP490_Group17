using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkShopSurveyQuestion
{
    public int Id { get; set; }

    public int WorkShopId { get; set; }

    public string? QuestionText { get; set; }

    public int QuestionTypeId { get; set; }

    public virtual ICollection<AnswerSurvey> AnswerSurveys { get; set; } = new List<AnswerSurvey>();

    public virtual WorkShopSurveyQuestionType QuestionType { get; set; } = null!;

    public virtual ICollection<SurveyAnswerDetail> SurveyAnswerDetails { get; set; } = new List<SurveyAnswerDetail>();

    public virtual Workshop WorkShop { get; set; } = null!;

    public virtual ICollection<WorkShopSurveyQuestionTextDetail> WorkShopSurveyQuestionTextDetails { get; set; } = new List<WorkShopSurveyQuestionTextDetail>();
}
