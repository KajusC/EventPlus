using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class EventLocation
{
    public int IdEventLocation { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public int? Capacity { get; set; }

    public string? Contacts { get; set; }

    public double? Price { get; set; }

    public int? TurimaÄæranga { get; set; }

    public virtual Event? Event { get; set; }

    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();

    public virtual Equipment? TurimaÄærangaNavigation { get; set; }
}
