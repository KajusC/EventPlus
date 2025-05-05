using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlus.Server.Application.ViewModels
{
    public class UserViewModel
    {
        public int IdUser { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? LastLogin { get; set; }
    }

    public class OrganiserViewModel : UserViewModel
    {
        public int? FollowerCount { get; set; }
        public double? Rating { get; set; }
    }

    public class AdministratorViewModel : UserViewModel
    {
        // Any administrator-specific properties would go here
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string UserType { get; set; } // "User", "Organiser", or "Administrator"
    }
}