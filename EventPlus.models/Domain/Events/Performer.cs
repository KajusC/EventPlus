using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Events;

public partial class Performer
{
    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Profession { get; set; }

    public int IdPerformer { get; set; }
}
