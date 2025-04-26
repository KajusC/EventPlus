using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Feedbacks;

public partial class AdministratorFeedback
{
    public int FkAdministratoridUser { get; set; }

    public int FkFeedbackidFeedback { get; set; }

    public virtual Administrator FkAdministratoridUserNavigation { get; set; } = null!;

    public virtual Feedback FkFeedbackidFeedbackNavigation { get; set; } = null!;
}
