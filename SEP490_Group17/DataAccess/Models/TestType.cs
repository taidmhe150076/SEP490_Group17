using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class TestType
{
    public int Id { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
