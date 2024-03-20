using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class QuestionParticipantsDetail
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public int AnswerId { get; set; }

    public DateTime Date { get; set; }

    public string? ParticipantsEmail { get; set; }

    public virtual AnswerQuestion Answer { get; set; } = null!;

    public virtual WorkshopQuestion Question { get; set; } = null!;
}
