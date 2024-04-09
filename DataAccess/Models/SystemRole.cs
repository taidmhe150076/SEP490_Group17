using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SystemRole
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<SystemUser> SystemUsers { get; set; } = new List<SystemUser>();
}
