using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Presenter
{
    public int PresenterId { get; set; }

    public string? PresenterEmail { get; set; }

    public virtual ICollection<Workshop> Workshops { get; set; } = new List<Workshop>();
}
