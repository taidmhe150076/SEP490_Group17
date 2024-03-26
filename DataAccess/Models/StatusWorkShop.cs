using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class StatusWorkShop
{
    public int StatusId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
}
