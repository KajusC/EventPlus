using eventplus.models.Domain.Users;
using EventPlus.Server.Application.ViewModels;
using System.Threading.Tasks;

namespace EventPlus.Server.Application.Authentication
{
    public interface IRoleBasedAuthService
    {
        Task<AuthResult> AuthenticateAsync(string username, string password);
        Task<AuthResult> RegisterUserAsync(UserViewModel userViewModel);
        Task<AuthResult> RegisterOrganiserAsync(OrganiserViewModel organiserViewModel);
        Task<AuthResult> RegisterAdministratorAsync(AdministratorViewModel administratorViewModel);
        Task<bool> ChangePasswordAsync(int userId, string userType, string currentPassword, string newPassword);
        Task<bool> UpdateLastLoginAsync(int userId, string userType);
    }
}