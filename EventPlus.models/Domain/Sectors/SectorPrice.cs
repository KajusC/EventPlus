using eventplus.models.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Sectors;

public partial class SectorPrice
{
    public double? Price { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSectorPrice { get; set; }

    public int FkSectoridSector { get; set; }

    public int FkEventidEvent { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual Sector FkSectoridSectorNavigation { get; set; } = null!;
}
