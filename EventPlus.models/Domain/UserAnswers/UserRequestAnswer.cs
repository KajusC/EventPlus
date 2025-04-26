using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserAnswers;

public partial class UserRequestAnswer
{
    public string? Answer { get; set; }

    public int IdUserRequestAnswer { get; set; }

    public int FkQuestionidQuestion { get; set; }

    public virtual Question FkQuestionidQuestionNavigation { get; set; } = null!;

    public virtual UserRequestAnswerAdministrator? UserRequestAnswerAdministrator { get; set; }

    public virtual UserRequestAnswerOrganiser? UserRequestAnswerOrganiser { get; set; }

    public virtual UserRequestAnswerUser? UserRequestAnswerUser { get; set; }
}
