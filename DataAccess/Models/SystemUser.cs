using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SystemUser
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public DateOnly? Dob { get; set; }

    public int? Departmentld { get; set; }

    public int? Roleld { get; set; }

    public string? Password { get; set; }

    public DateOnly? ValidDate { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Assign> Assigns { get; set; } = new List<Assign>();

    public virtual Department? DepartmentldNavigation { get; set; }

    public virtual SystemRole? RoleldNavigation { get; set; }
}
