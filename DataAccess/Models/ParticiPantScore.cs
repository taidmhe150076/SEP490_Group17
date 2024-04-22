using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ParticiPantScore
{
    public int TestId { get; set; }

    public int ParticipantId { get; set; }

    public double Score { get; set; }

    public DateTime? SubmissionTime { get; set; }

    public virtual ParticipantAnswer Participant { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
