using eventplus.models.Domain.Events;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
    public interface IEventLogic
    {
        Task<List<EventViewModel>> GetAllEventsAsync();
        Task<EventViewModel> GetEventByIdAsync(int id);
        Task<int> CreateEventAsync(EventViewModel eventEntity);
        Task<bool> UpdateEventAsync(EventViewModel eventEntity);
        Task<bool> DeleteEventAsync(int id);

        Task<EventLocation> CreateEventLocation(EventLocationViewModel eventLocation);
        Task<Partner> CreatePartners(PartnerViewModel eventPartners);
        Task<Performer> CreatePerformers(PerformerViewModel eventPerformers);

        Task<int> CreateFullEvent(EventViewModel eventViewModel,
            EventLocationViewModel eventLocation,
            PartnerViewModel partner,
            PerformerViewModel performer);
    }
}
