using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ImageType
{
    public int Id { get; set; }

    public string? ImageType1 { get; set; }

    public virtual ICollection<ImagesWorkShop> ImagesWorkShops { get; set; } = new List<ImagesWorkShop>();
}
