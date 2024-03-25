using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ParticipantAnswer
{
    public int Id { get; set; }

    public string? ParticipantsEmail { get; set; }

    public virtual ICollection<AnswerParticipant> AnswerParticipants { get; set; } = new List<AnswerParticipant>();

    public virtual ICollection<ParticiPantScore> ParticiPantScores { get; set; } = new List<ParticiPantScore>();
}
