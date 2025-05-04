using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.Sectors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Events;

public partial class EventLocation
{
    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public int? Capacity { get; set; }

    public string? Contacts { get; set; }

    public double? Price { get; set; }

    public int? HoldingEquipment { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEventLocation { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Equipment? HoldingEquipmentNavigation { get; set; }

    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
