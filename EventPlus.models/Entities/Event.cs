using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Event
{
    public int IdEvent { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? MaxTicketCount { get; set; }

    public int? Category { get; set; }

    public int FkEventLocationidEventLocation { get; set; }

    public int FkOrganiseridUser { get; set; }

    public virtual Category? CategoryNavigation { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual EventLocation FkEventLocationidEventLocationNavigation { get; set; } = null!;

    public virtual Organiser FkOrganiseridUserNavigation { get; set; } = null!;

    public virtual EventPerformer? RenginioatlikÄJa { get; set; }

    public virtual EventSponsors? Renginiopartneri { get; set; }

    public virtual ICollection<SectorPrice> SectorPrices { get; set; } = new List<SectorPrice>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
