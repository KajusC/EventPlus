using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Partner
{
    public int IdPartner { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }
}
