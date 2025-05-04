using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Events;

public partial class Partner
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPartner { get; set; }
}
