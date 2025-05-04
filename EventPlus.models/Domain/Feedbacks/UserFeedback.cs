using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Feedbacks;

public partial class UserFeedback
{
    public int FkUseridUser { get; set; }

    public int FkFeedbackidFeedback { get; set; }

    public virtual Feedback FkFeedbackidFeedbackNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;
}
