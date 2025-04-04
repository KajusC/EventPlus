using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Equipment
{
    public int IdEquipment { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<EventLocation> EventLocations { get; set; } = new List<EventLocation>();
}
