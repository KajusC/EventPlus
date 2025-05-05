using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.UserLoyalties;
using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EventPlusContext context) : base(context)
        {
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            if (!ValidateUser(username, password))
            {
                return null;
            }

            var user = await _dbSet.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async Task<UserLoyalty?> GetUserLoyaltyAsync(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            var userLoyalty = await _context.UserLoyalties.FirstOrDefaultAsync(ul => ul.FkUseridUser == userId);
            return userLoyalty;
        }

        public async Task<List<UserTicket>> GetUserTicketsAsync(int userId)
        {
            if (userId <= 0)
            {
                return new List<UserTicket>();
            }
            var userTickets = await _context.UserTickets.Where(ut => ut.FkUseridUser == userId).ToListAsync();
            return userTickets;
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
            return user == null;
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<User>();
            }
            var users = await _dbSet.Where(u => u.Username.Contains(searchTerm)).ToListAsync();
            return users;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }
            var user = await GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.LastLogin = DateTime.Now;
            _dbSet.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        private bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            return true;
        }
    }
}
