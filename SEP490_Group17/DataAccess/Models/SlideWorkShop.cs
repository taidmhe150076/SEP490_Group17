using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class SlideWorkShop
{
    public int Id { get; set; }

    public int ImageId { get; set; }

    public int WorkshopId { get; set; }

    public virtual Image Image { get; set; } = null!;

    public virtual Workshop Workshop { get; set; } = null!;
}
