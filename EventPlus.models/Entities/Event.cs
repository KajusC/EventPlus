using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public class Event
{
    public int IdEvent { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MaxTicketCount { get; set; }

    public int? Category { get; set; }

    public int FkEventLocationidEventLocation { get; set; }

    public int FkOrganiseridUser { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual EventLocation FkEventLocationidEventLocationNavigation { get; set; } = null!;

    public virtual Organiser FkOrganiseridUserNavigation { get; set; } = null!;

    public virtual EventPerformer? EventPerformer { get; set; }

    public virtual EventPartner? EventPartner { get; set; }

    public virtual ICollection<SectorPrice> SectorPrices { get; set; } = new List<SectorPrice>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
