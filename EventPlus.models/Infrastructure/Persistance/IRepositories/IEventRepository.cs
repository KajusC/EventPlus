using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<List<Event>> GetEventsByUserIdAsync(int userId);
        Task<List<Event>> GetEventsByCategoryIdAsync(int categoryId);
        Task<List<Event>> GetEventsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<List<Event>> GetEventsByUserTicketsAsync(int userId);
        Task<List<Event>> GetEventsByOrganiserIdAsync(int organiserId);
        Task<List<Event>> GetEventsByLocationIdAsync(int locationId);
    }
}
