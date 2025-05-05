using eventplus.models.Domain.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IAdministratorRepository : IRepository<Administrator>
    {
        Task<Administrator?> GetByUsernameAsync(string username);
        Task<Administrator?> AuthenticateAsync(string username, string password);
        Task<bool> IsUsernameUniqueAsync(string username);
        Task<bool> UpdateLastLoginAsync(int userId);
    }
}