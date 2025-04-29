using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
    public class EventLogic : IEventLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EventLogic(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateEventAsync(EventViewModel eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            // Due to model entity requiring location, user, category, create fake dummy data inside database

            var eventEntityMapped = _mapper.Map<Event>(eventEntity);
            await _unitOfWork.Events.CreateAsync(eventEntityMapped);
            return eventEntityMapped.IdEvent;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _unitOfWork.Events.DeleteAsync(id);
        }

        public async Task<List<EventViewModel>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            return _mapper.Map<List<EventViewModel>>(events);
        }

        public async Task<EventViewModel> GetEventByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            return _mapper.Map<EventViewModel>(eventEntity);
        }

        public async Task<bool> UpdateEventAsync(EventViewModel eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            if (!CheckEventFormData(eventEntity))
            {
                return false;
            }

            var eventEntityMapped = _mapper.Map<Event>(eventEntity);
            return await _unitOfWork.Events.UpdateAsync(eventEntityMapped);
        }

        private static bool CheckEventFormData(EventViewModel eventDTO)
        {
            if (string.IsNullOrWhiteSpace(eventDTO.Name))
            {
                return false;
            }
            if (eventDTO.StartDate == null || eventDTO.EndDate == null)
            {
                return false;
            }
            if (eventDTO.StartDate >= eventDTO.EndDate)
            {
                return false;
            }

            if (eventDTO.MaxTicketCount < 0)
            {
                return false;
            }

            return true;
        }

        // TODO - Might have potential re-adding
        public async Task<int> CreateFullEvent(
            EventViewModel eventViewModel,
            EventLocationViewModel eventLocation,
            PartnerViewModel partner,
            PerformerViewModel performer)
        {
            if (eventViewModel == null)
            {
                throw new ArgumentNullException(nameof(eventViewModel));
            }

            if (eventLocation == null)
            {
                throw new ArgumentNullException(nameof(eventLocation));
            }

            if (partner == null)
            {
                throw new ArgumentNullException(nameof(partner));
            }

            if (performer == null)
            {
                throw new ArgumentNullException(nameof(performer));
            }

            // Validate event data
            if (!CheckEventFormData(eventViewModel))
            {
                throw new ArgumentException("Invalid event data.");
            }

            try
            {
                var locationCreated = await CreateEventLocation(eventLocation);
                if (locationCreated == null)
                {
                    throw new Exception("Failed to create event location.");
                }
                eventViewModel.FkEventLocationidEventLocation = locationCreated.IdEventLocation;

                var eventCreated = await CreateEventAsync(eventViewModel);

                if (eventCreated > 0)
                {
                    throw new Exception("Failed to create event.");
                }

                int eventId = eventCreated;

                var partnerMapped = _mapper.Map<Partner>(partner);
                await _unitOfWork.Partners.CreateAsync(partnerMapped);


                var performerMapped = _mapper.Map<Performer>(performer);
                await _unitOfWork.Performers.CreateAsync(performerMapped);

                // Save all changes
                await _unitOfWork.SaveAsync();

                return eventId;
            }
            catch (Exception)
            {
                // Handle any exceptions and return failure
                return -1;
            }
        }

        public async Task<EventLocation> CreateEventLocation(EventLocationViewModel eventLocation)
        {
            if (eventLocation == null)
            {
                throw new ArgumentNullException(nameof(eventLocation));
            }
            var eventLocationMapped = _mapper.Map<EventLocation>(eventLocation);
            await _unitOfWork.EventLocations.CreateAsync(eventLocationMapped);
            return eventLocationMapped;
        }

        public async Task<Partner> CreatePartners(PartnerViewModel eventPartners)
        {
            if (eventPartners == null)
            {
                throw new ArgumentNullException(nameof(eventPartners));
            }
            var eventPartnersMapped = _mapper.Map<Partner>(eventPartners);
            await _unitOfWork.Partners.CreateAsync(eventPartnersMapped);
            return eventPartnersMapped;
        }

        public async Task<Performer> CreatePerformers(PerformerViewModel eventPerformers)
        {
            if (eventPerformers == null)
            {
                throw new ArgumentNullException(nameof(eventPerformers));
            }
            var eventPerformersMapped = _mapper.Map<Performer>(eventPerformers);
            await _unitOfWork.Performers.CreateAsync(eventPerformersMapped);
            return eventPerformersMapped;
        }
    }
}
