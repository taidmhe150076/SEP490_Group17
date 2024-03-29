using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class WorkshopQuestion
{
    public int Id { get; set; }

    public int? WorkshopId { get; set; }

    public string? QuestionText { get; set; }

    public virtual ICollection<AnswerParticipant> AnswerParticipants { get; set; } = new List<AnswerParticipant>();

    public virtual ICollection<AnswerQuestion> AnswerQuestions { get; set; } = new List<AnswerQuestion>();

    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();

    public virtual Workshop? Workshop { get; set; }
}
