using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserAnswers;

public partial class UserRequestAnswerOrganiser
{
    public int FkUserRequestAnsweridUserRequestAnswer { get; set; }

    public int FkOrganiseridUser { get; set; }

    public virtual Organiser FkOrganiseridUserNavigation { get; set; } = null!;

    public virtual UserRequestAnswer FkUserRequestAnsweridUserRequestAnswerNavigation { get; set; } = null!;
}
