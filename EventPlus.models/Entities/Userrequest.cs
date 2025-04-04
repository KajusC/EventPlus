using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Userrequest
{
    public int FkUseridUser { get; set; }

    public virtual User FkUseridUserNavigation { get; set; } = null!;
}
