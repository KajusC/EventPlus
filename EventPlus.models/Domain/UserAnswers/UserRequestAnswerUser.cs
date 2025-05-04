using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserAnswers;

public partial class UserRequestAnswerUser
{
    public int FkUserRequestAnsweridUserRequestAnswer { get; set; }

    public int FkUseridUser { get; set; }

    public virtual UserRequestAnswer FkUserRequestAnsweridUserRequestAnswerNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;
}
