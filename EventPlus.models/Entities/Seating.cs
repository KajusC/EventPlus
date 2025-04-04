using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Seating
{
    public int IdSeating { get; set; }

    public int? Row { get; set; }

    public int? Place { get; set; }

    public int FkSectoridSector { get; set; }

    public int FkSectorfkEventLocationidEventLocation { get; set; }

    public int FkTicketidTicket { get; set; }

    public virtual Ticket FkTicketidTicketNavigation { get; set; } = null!;

    public virtual Sector Sector { get; set; } = null!;
}
