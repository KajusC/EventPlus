using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Loyalty
{
    public int IdLoyalty { get; set; }

    public int? Points { get; set; }

    public virtual User? User { get; set; }
}
