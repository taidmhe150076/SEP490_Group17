using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AnswerSurvey
{
    public int Id { get; set; }

    public int? WorkShopSurveyQuestionId { get; set; }

    public int? Level { get; set; }

    public virtual ICollection<SurveyAnswerDetail> SurveyAnswerDetails { get; set; } = new List<SurveyAnswerDetail>();

    public virtual WorkShopSurveyQuestion? WorkShopSurveyQuestion { get; set; }
}
