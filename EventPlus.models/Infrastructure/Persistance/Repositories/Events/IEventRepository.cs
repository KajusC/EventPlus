using eventplus.models.Domain.Events;

namespace eventplus.models.Infrastructure.Persistance.Repositories.Events
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task<bool> CreateEventAsync(Event eventEntity);
        Task<bool> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int id);
        Task<List<Event>> GetEventsByUserIdAsync(int userId);
        Task<List<Event>> GetEventsByCategoryIdAsync(int categoryId);
        Task<List<Event>> GetEventsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}
