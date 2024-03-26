using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Test
{
    public int Id { get; set; }

    public string? TestName { get; set; }

    public DateTime? DateStart { get; set; }

    public int? WorkshopId { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public string? QrTest { get; set; }

    public virtual ICollection<AnswerParticipant> AnswerParticipants { get; set; } = new List<AnswerParticipant>();

    public virtual ICollection<ParticiPantScore> ParticiPantScores { get; set; } = new List<ParticiPantScore>();

    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();

    public virtual Workshop? Workshop { get; set; }
}
