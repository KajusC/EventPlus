using eventplus.models.Domain.Events;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Sectors;

public partial class Sector
{
    public string? Name { get; set; }

    public int IdSector { get; set; }

    public int FkEventLocationidEventLocation { get; set; }

    public virtual EventLocation FkEventLocationidEventLocationNavigation { get; set; } = null!;

    public virtual ICollection<Seating> Seatings { get; set; } = new List<Seating>();

    public virtual ICollection<SectorPrice> SectorPrices { get; set; } = new List<SectorPrice>();
}
