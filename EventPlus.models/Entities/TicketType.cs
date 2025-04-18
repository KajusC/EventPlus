﻿using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class TicketType
{
    public int IdTicketType { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
