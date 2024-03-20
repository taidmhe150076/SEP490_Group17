using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkshopQuestion
{
    public int Id { get; set; }

    public int? WorkshopId { get; set; }

    public string? QuestionText { get; set; }

    public virtual ICollection<AnswerQuestion> AnswerQuestions { get; set; } = new List<AnswerQuestion>();

    public virtual ICollection<QuestionParticipantsDetail> QuestionParticipantsDetails { get; set; } = new List<QuestionParticipantsDetail>();

    public virtual Workshop? Workshop { get; set; }
}
