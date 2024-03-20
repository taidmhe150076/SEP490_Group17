using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Participant
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public int? WorkshopId { get; set; }

    public virtual Workshop? Workshop { get; set; }
}
