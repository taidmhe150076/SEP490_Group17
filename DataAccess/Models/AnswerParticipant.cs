using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class AnswerParticipant
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public int TestId { get; set; }

    public int AnswerId { get; set; }

    public int? ParticipantId { get; set; }

    public DateTime? SubmissionTime { get; set; }

    public virtual AnswerQuestion Answer { get; set; } = null!;

    public virtual ParticipantAnswer? Participant { get; set; }

    public virtual WorkshopQuestion Question { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
