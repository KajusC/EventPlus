using eventplus.models.Domain.Users;
using EventPlus.Server.Application.ViewModels;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> AuthenticateAsync(string username, string password);
        Task<AuthResult> RegisterAsync(UserViewModel user);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> UpdateLastLoginAsync(int userId);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        
        // New properties for role-based authentication
        public int UserId { get; set; } 
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; } // "User", "Organiser", or "Administrator"
    }
}