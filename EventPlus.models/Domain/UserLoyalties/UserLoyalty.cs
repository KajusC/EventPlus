using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserLoyalties;

public partial class UserLoyalty
{
    public int FkUseridUser { get; set; }

    public int FkLoyaltyidLoyalty { get; set; }

    public virtual Loyalty FkLoyaltyidLoyaltyNavigation { get; set; } = null!;

    public virtual User FkUseridUserNavigation { get; set; } = null!;
}
