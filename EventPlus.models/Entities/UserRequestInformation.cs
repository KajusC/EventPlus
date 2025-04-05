using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class UserRequestInformation
{
    public int IdUserRequestInformation { get; set; }

    public string? Question { get; set; }

    public string? Response { get; set; }
}
