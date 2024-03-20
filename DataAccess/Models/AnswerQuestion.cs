using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AnswerQuestion
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public string AnswerText { get; set; } = null!;

    public bool IsCorrectAnswer { get; set; }

    public virtual WorkshopQuestion Question { get; set; } = null!;

    public virtual ICollection<QuestionParticipantsDetail> QuestionParticipantsDetails { get; set; } = new List<QuestionParticipantsDetail>();
}
