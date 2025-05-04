using eventplus.models.Domain.Feedbacks;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.Users;

public partial class OrganiserFeedback
{
    public int FkOrganiseridUser { get; set; }

    public int FkFeedbackidFeedback { get; set; }

    public virtual Feedback FkFeedbackidFeedbackNavigation { get; set; } = null!;

    public virtual Organiser FkOrganiseridUserNavigation { get; set; } = null!;
}
