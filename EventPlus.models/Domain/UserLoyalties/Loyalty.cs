using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.UserLoyalties;

public partial class Loyalty
{
    public int? Points { get; set; }

    public DateOnly? Date { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdLoyalty { get; set; }

    public virtual AdministratorLoyalty? AdministratorLoyalty { get; set; }

    public virtual OrganiserLoyalty? OrganiserLoyalty { get; set; }

    public virtual UserLoyalty? UserLoyalty { get; set; }
}
