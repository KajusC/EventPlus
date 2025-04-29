using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.UserLoyalties;
using eventplus.models.Domain.Users;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<List<User>> SearchUsersAsync(string searchTerm);
        Task<bool> UpdateLastLoginAsync(int userId);
        Task<List<UserTicket>> GetUserTicketsAsync(int userId);
        Task<UserLoyalty?> GetUserLoyaltyAsync(int userId);
    }
}
