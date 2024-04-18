using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public DateTime? Dob { get; set; }

    public int? RoleId { get; set; }

    public int? DepartmentId { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Role? Role { get; set; }
}
