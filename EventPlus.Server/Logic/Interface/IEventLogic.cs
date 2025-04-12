using eventplus.models.Entities;
using EventPlus.Server.DTO;

namespace EventPlus.Server.Logic.Interface
{
    public interface IEventLogic
    {
        Task<List<EventDTO>> GetAllEventsAsync();
        Task<EventDTO> GetEventByIdAsync(int id);
        Task<bool> CreateEventAsync(EventDTO eventEntity);
        Task<bool> UpdateEventAsync(EventDTO eventEntity);
        Task<bool> DeleteEventAsync(int id);
    }
}
