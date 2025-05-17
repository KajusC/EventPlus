using eventplus.models.Domain.Users;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class OrganiserRepository : Repository<Organiser>, IOrganiserRepository
    {
        public OrganiserRepository(EventPlusContext context) : base(context)
        {
        }

        public async Task<Organiser?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            var organiser = await _dbSet.FirstOrDefaultAsync(o => o.Username == username && o.Password == password);
            return organiser;
        }

        public async Task<Organiser?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            var organiser = await _dbSet.FirstOrDefaultAsync(o => o.Username == username);
            return organiser;
        }

        public override async Task<Organiser> GetByIdAsync(int id)
        {
            var organiser = await _dbSet.FirstOrDefaultAsync(o => o.IdUser == id);
            return organiser;
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }
            var organiser = await _dbSet.FirstOrDefaultAsync(o => o.Username == username);
            return organiser == null;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            if (userId <= 0)
            {
                return false;
            }

            var organiser = await _dbSet.FirstOrDefaultAsync(o => o.IdUser == userId);
            if (organiser == null)
            {
                return false;
            }

            organiser.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}