using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.Users;
using eventplus.models.Domain.Feedbacks;
using System;
using System.Collections.Generic;
using eventplus.models.Domain.Sectors;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Events;

public partial class Event
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? MaxTicketCount { get; set; }

    public int? Category { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEvent { get; set; }

    public int FkEventLocationidEventLocation { get; set; }

    public int FkOrganiseridUser { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual EventLocation FkEventLocationidEventLocationNavigation { get; set; } = null!;

    public virtual Organiser FkOrganiseridUserNavigation { get; set; } = null!;

    public virtual ICollection<SectorPrice> SectorPrices { get; set; } = new List<SectorPrice>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
