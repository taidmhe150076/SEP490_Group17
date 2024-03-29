using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class TestQuestion
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public int TestId { get; set; }

    public DateTime? Date { get; set; }

    public virtual WorkshopQuestion Question { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
