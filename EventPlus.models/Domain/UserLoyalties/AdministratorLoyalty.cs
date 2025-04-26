using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;

namespace eventplus.models.Domain.UserLoyalties;

public partial class AdministratorLoyalty
{
    public int FkAdministratoridUser { get; set; }

    public int FkLoyaltyidLoyalty { get; set; }

    public virtual Administrator FkAdministratoridUserNavigation { get; set; } = null!;

    public virtual Loyalty FkLoyaltyidLoyaltyNavigation { get; set; } = null!;
}
