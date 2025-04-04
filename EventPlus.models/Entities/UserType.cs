using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class UserType
{
    public int IdUserType { get; set; }

    public string Name { get; set; } = null!;
}
