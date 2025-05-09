using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
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
		Task<Sector> CreateSectors(SectorViewModel eventPartners);

		Task<int> CreateFullEvent(EventViewModel eventViewModel,
            EventLocationViewModel eventLocation,
            PartnerViewModel partner,
            PerformerViewModel performer,
            SectorViewModel sector,
            List<SectorViewModel> sectors,
			List<SectorPriceViewModel> sectorPrice,
			List<SeatingViewModel> seating);
    }
}
