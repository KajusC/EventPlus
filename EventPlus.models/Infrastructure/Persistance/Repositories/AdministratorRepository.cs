using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class AdministratorRepository : Repository<Administrator>, IAdministratorRepository
    {
        public AdministratorRepository(EventPlusContext context) : base(context)
        {
        }

        public async Task<Administrator?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            var admin = await _dbSet.FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
            return admin;
        }

        public async Task<Administrator?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            var admin = await _dbSet.FirstOrDefaultAsync(a => a.Username == username);
            return admin;
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }
            var admin = await _dbSet.FirstOrDefaultAsync(a => a.Username == username);
            return admin == null;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }

            var admin = await _dbSet.FirstOrDefaultAsync(a => a.IdUser == userId);
            if (admin == null)
            {
                return false;
            }

            admin.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}