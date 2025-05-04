using eventplus.models.Domain.Tickets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Sectors;

public partial class Seating
{
    public int? Row { get; set; }

    public int? Place { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdSeating { get; set; }

    public int? FkSectoridSector { get; set; }

    public virtual Sector? FkSectoridSectorNavigation { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
