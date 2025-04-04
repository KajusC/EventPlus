using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class Feedback
{
    public int IdFeedback { get; set; }

    public string? Comment { get; set; }

    public double? Vote { get; set; }

    public int? Type { get; set; }

    public int FkEventidEvent { get; set; }

    public int FkUseridUser { get; set; }

    public virtual Event FkEventidEventNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;

    public virtual FeedbackType? TypeNavigation { get; set; }
}
