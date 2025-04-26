using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserAnswers;

public partial class UserRequestAnswerAdministrator
{
    public int FkUserRequestAnsweridUserRequestAnswer { get; set; }

    public int FkAdministratoridUser { get; set; }

    public virtual Administrator FkAdministratoridUserNavigation { get; set; } = null!;

    public virtual UserRequestAnswer FkUserRequestAnsweridUserRequestAnswerNavigation { get; set; } = null!;
}
