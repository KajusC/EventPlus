using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Organiser
{
    public int IdUser { get; set; }

    public int? FollowerAmount { get; set; }

    public double? Rating { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual User IdUserNavigation { get; set; } = null!;
}
