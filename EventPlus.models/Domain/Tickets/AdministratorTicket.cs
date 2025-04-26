using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Tickets;

public partial class AdministratorTicket
{
    public int FkAdministratoridUser { get; set; }

    public int FkTicketidTicket { get; set; }

    public virtual Administrator FkAdministratoridUserNavigation { get; set; } = null!;

    public virtual Ticket FkTicketidTicketNavigation { get; set; } = null!;
}
