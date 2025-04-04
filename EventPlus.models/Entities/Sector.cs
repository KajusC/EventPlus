using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Sector
{
    public int IdSector { get; set; }

    public int FkEventLocationidEventLocation { get; set; }

    public string? Name { get; set; }

    public virtual EventLocation FkEventLocationidEventLocationNavigation { get; set; } = null!;

    public virtual ICollection<Seating> Seatings { get; set; } = new List<Seating>();

    public virtual ICollection<SectorPrice> SectorPrices { get; set; } = new List<SectorPrice>();
}
