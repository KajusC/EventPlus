using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Tickets;

public partial class Ticketstatus
{
    public int IdStatus { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
