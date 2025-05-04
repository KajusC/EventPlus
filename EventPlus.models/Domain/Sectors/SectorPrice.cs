using eventplus.models.Domain.Events;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Sectors;

public partial class SectorPrice
{
    public double? Price { get; set; }

    public int IdSectorPrice { get; set; }

    public int FkSectoridSector { get; set; }

    public int FkEventidEvent { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual Sector FkSectoridSectorNavigation { get; set; } = null!;
}
