using eventplus.models.Domain.Tickets;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Users;

public partial class OrganiserTicket
{
    public int FkOrganiseridUser { get; set; }

    public int FkTicketidTicket { get; set; }

    public virtual Organiser FkOrganiseridUserNavigation { get; set; } = null!;

    public virtual Ticket FkTicketidTicketNavigation { get; set; } = null!;
}
