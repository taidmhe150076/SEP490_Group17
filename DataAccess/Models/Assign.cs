using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Assign
{
    public int Id { get; set; }

    public int WorkshopSeriesId { get; set; }

    public int UserSystemId { get; set; }

    public virtual SystemUser UserSystem { get; set; } = null!;

    public virtual WorkshopSeries WorkshopSeries { get; set; } = null!;
}
