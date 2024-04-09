using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Department
{
    public int Id { get; set; }

    public string? DepartmentName { get; set; }

    public virtual ICollection<SystemUser> SystemUsers { get; set; } = new List<SystemUser>();

    public virtual ICollection<WorkshopSeries> WorkshopSeries { get; set; } = new List<WorkshopSeries>();
}
