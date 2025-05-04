using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Events;

public partial class Equipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEquipment { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<EventLocation> EventLocations { get; set; } = new List<EventLocation>();
}
