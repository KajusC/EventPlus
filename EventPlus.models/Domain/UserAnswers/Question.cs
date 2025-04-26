using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserAnswers;

public partial class Question
{
    public string? FormulatedQuestion { get; set; }

    public int IdQuestion { get; set; }

    public int? FkAdministratoridUser { get; set; }

    public virtual Administrator? FkAdministratoridUserNavigation { get; set; }

    public virtual ICollection<UserRequestAnswer> UserRequestAnswers { get; set; } = new List<UserRequestAnswer>();
}
