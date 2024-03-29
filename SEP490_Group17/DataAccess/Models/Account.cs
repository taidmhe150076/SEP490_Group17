using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Account
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public DateTime? ValidDate { get; set; }

    public virtual User? User { get; set; }
}
