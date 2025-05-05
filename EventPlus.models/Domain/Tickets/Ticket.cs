using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Tickets;

public partial class Ticket
{
    public double? Price { get; set; }

    public DateOnly? GenerationDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ScannedDate { get; set; }

    public string? QrCode { get; set; }

    public int? Type { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdTicket { get; set; }

    public int FkEventidEvent { get; set; }

    public int FkSeatingidSeating { get; set; }

    public int FkTicketstatus { get; set; }

    public virtual AdministratorTicket? AdministratorTicket { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual Seating FkSeatingidSeatingNavigation { get; set; } = null!;

    public virtual Ticketstatus FkTicketstatusNavigation { get; set; } = null!;

    public virtual OrganiserTicket? OrganiserTicket { get; set; }

    public virtual TicketType? TypeNavigation { get; set; }

    public virtual UserTicket? UserTicket { get; set; }
}
