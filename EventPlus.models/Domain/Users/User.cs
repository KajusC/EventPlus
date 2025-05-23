﻿using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Domain.UserLoyalties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eventplus.models.Domain.Users;

public partial class User
{
    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Password { get; set; }

    public string? Username { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? LastLogin { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdUser { get; set; }

    public virtual ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();

    public virtual UserLoyalty? UserLoyalty { get; set; }

    public virtual ICollection<UserRequestAnswerUser> UserRequestAnswerUsers { get; set; } = new List<UserRequestAnswerUser>();

    public virtual ICollection<UserTicket> UserTickets { get; set; } = new List<UserTicket>();
}
