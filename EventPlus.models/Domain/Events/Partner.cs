using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Events;

public partial class Partner
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    public int IdPartner { get; set; }
}
