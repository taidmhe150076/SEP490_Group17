using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ChartImage
{
    public int Id { get; set; }

    public int ImageId { get; set; }

    public int WorkshopId { get; set; }

    public string? Descriptions { get; set; }

    public string? Title { get; set; }

    public virtual Image Image { get; set; } = null!;

    public virtual Workshop Workshop { get; set; } = null!;
}
