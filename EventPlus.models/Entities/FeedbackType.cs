using System;
using System.Collections.Generic;

namespace eventplus.models.Entities;

public partial class FeedbackType
{
    public int IdFeedbackType { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
