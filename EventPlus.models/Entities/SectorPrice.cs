using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class SectorPrice
{
    public int IdSectorPrice { get; set; }

    public double? Price { get; set; }

    public int FkSectoridSector { get; set; }

    public int FkSectorfkEventLocationidEventLocation { get; set; }

    public int FkEventidEvent { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual Sector Sector { get; set; } = null!;
}
