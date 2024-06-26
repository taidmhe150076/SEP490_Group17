﻿using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Image
{
    public int Id { get; set; }

    public string? Image1 { get; set; }

    public virtual ICollection<ImagesWorkShop> ImagesWorkShops { get; set; } = new List<ImagesWorkShop>();
}
