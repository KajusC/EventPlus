using eventplus.models.Domain.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IOrganiserRepository : IRepository<Organiser>
    {
        Task<Organiser?> GetByUsernameAsync(string username);
        Task<Organiser?> AuthenticateAsync(string username, string password);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> UpdateLastLoginAsync(int userId);
    }
}