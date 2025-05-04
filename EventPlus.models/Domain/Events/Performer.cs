using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Events;

public partial class Performer
{
    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Profession { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPerformer { get; set; }
}
