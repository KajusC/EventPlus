using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Domain.UserLoyalties;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Users;

public partial class Administrator
{
    public int IdUser { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Password { get; set; }

    public string? Username { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<AdministratorFeedback> AdministratorFeedbacks { get; set; } = new List<AdministratorFeedback>();

    public virtual AdministratorLoyalty? AdministratorLoyalty { get; set; }

    public virtual ICollection<AdministratorTicket> AdministratorTickets { get; set; } = new List<AdministratorTicket>();

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<UserRequestAnswerAdministrator> UserRequestAnswerAdministrators { get; set; } = new List<UserRequestAnswerAdministrator>();
}
