using eventplus.models.Domain.Events;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Domain.UserLoyalties;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Users;

public partial class Organiser
{
    public int? FollowerCount { get; set; }

    public double? Rating { get; set; }

    public int IdUser { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Password { get; set; }

    public string? Username { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<OrganiserFeedback> OrganiserFeedbacks { get; set; } = new List<OrganiserFeedback>();

    public virtual OrganiserLoyalty? OrganiserLoyalty { get; set; }

    public virtual ICollection<OrganiserTicket> OrganiserTickets { get; set; } = new List<OrganiserTicket>();

    public virtual ICollection<UserRequestAnswerOrganiser> UserRequestAnswerOrganisers { get; set; } = new List<UserRequestAnswerOrganiser>();
}
