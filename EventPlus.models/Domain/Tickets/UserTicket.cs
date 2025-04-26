using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Tickets;

public partial class UserTicket
{
    public int FkUseridUser { get; set; }

    public int FkTicketidTicket { get; set; }

    public virtual Ticket FkTicketidTicketNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;
}
