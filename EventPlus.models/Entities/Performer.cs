using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Performer
{
    public int IdPerformer { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Profesija { get; set; }
}
