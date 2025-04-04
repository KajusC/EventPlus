using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class User
{
    public int IdUser { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Password { get; set; }

    public string? Username { get; set; }

    public DateOnly? LastLogin { get; set; }

    public int FkLoyaltyidLoyalty { get; set; }

    public virtual Administrator? Administrator { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual Loyalty FkLoyaltyidLoyaltyNavigation { get; set; } = null!;

    public virtual Organiser? Organiser { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Userrequest? Userrequest { get; set; }
}
