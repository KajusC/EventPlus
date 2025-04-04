using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Administrator
{
    public int IdUser { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;
}
