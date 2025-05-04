using eventplus.models.Domain.Events;
using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Feedbacks;

public partial class Feedback
{
    public string? Comment { get; set; }

    public double? Score { get; set; }

    public int? Type { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdFeedback { get; set; }

    public int FkEventidEvent { get; set; }

    public virtual AdministratorFeedback? AdministratorFeedback { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual OrganiserFeedback? OrganiserFeedback { get; set; }

    public virtual FeedbackType? TypeNavigation { get; set; }

    public virtual UserFeedback? UserFeedback { get; set; }
}
