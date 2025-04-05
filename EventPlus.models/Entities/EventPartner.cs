using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class EventPartner
{
    public int FkEventidEvent { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;
}
