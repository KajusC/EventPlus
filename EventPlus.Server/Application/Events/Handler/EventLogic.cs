using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.Persistance.Repositories.Events;
using EventPlus.Server.Application.Events.ViewModel;

namespace EventPlus.Server.Application.Events.Handler
{
    public class EventLogic : IEventLogic
    {

        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventLogic(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateEventAsync(EventViewModel eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            // Due to model entity requiring location, user, category, create fake dummy data inside database

            var eventEntityMapped = _mapper.Map<Event>(eventEntity);
            return await _eventRepository.CreateEventAsync(eventEntityMapped);
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _eventRepository.DeleteEventAsync(id);
        }

        public async Task<List<EventViewModel>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return _mapper.Map<List<EventViewModel>>(events);
        }

        public async Task<EventViewModel> GetEventByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var eventEntity = await _eventRepository.GetEventByIdAsync(id);
            return _mapper.Map<EventViewModel>(eventEntity);
        }

        public async Task<bool> UpdateEventAsync(EventViewModel eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            if (!ValidateEventData(eventEntity))
            {
                return false;
            }

            var eventEntityMapped = _mapper.Map<Event>(eventEntity);
            return await _eventRepository.UpdateEventAsync(eventEntityMapped);
        }

        private static bool ValidateEventData(EventViewModel eventDTO)
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
    }
}
