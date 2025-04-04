using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Category
{
    public int IdCategory { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
