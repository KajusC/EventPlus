using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Users;

public partial class UserType
{
    public int IdUserType { get; set; }

    public string Name { get; set; } = null!;
}
