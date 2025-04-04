using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class UserRequestInformation
{
    public int IdUserRequestInformation { get; set; }

    public string? Klausimas { get; set; }

    public string? Atsakas { get; set; }
}
