using EventPlus.Server.Application.Events.ViewModel;

namespace EventPlus.Server.Application.Events.Handler
{
    public interface IEventLogic
    {
        Task<List<EventViewModel>> GetAllEventsAsync();
        Task<EventViewModel> GetEventByIdAsync(int id);
        Task<bool> CreateEventAsync(EventViewModel eventEntity);
        Task<bool> UpdateEventAsync(EventViewModel eventEntity);
        Task<bool> DeleteEventAsync(int id);
    }
}
