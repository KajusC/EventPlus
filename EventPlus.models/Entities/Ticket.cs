using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Ticket
{
    public int IdTicket { get; set; }

    public double? Price { get; set; }

    public DateOnly? GenerationDate { get; set; }

    public string? QrCode { get; set; }

    public int? Type { get; set; }

    public int FkUseridUser { get; set; }

    public int FkEventidEvent { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;

    public virtual Seating? Seating { get; set; }

    public virtual TicketType? TypeNavigation { get; set; }
}
