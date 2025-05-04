using eventplus.models.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.UserAnswers;

public partial class Question
{
    public string? FormulatedQuestion { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdQuestion { get; set; }

    public int? FkAdministratoridUser { get; set; }

    public virtual Administrator? FkAdministratoridUserNavigation { get; set; }

    public virtual ICollection<UserRequestAnswer> UserRequestAnswers { get; set; } = new List<UserRequestAnswer>();
}
