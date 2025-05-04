using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.UserAnswers;

public partial class UserRequestAnswer
{
    public string? Answer { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdUserRequestAnswer { get; set; }

    public int FkQuestionidQuestion { get; set; }

    public virtual Question FkQuestionidQuestionNavigation { get; set; } = null!;

    public virtual UserRequestAnswerAdministrator? UserRequestAnswerAdministrator { get; set; }

    public virtual UserRequestAnswerOrganiser? UserRequestAnswerOrganiser { get; set; }

    public virtual UserRequestAnswerUser? UserRequestAnswerUser { get; set; }
}
