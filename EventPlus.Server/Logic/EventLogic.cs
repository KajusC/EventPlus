﻿using AutoMapper;
using eventplus.models.Repository.EventRepository;
using EventPlus.Server.DTO;
using EventPlus.Server.Logic.Interface;

namespace EventPlus.Server.Logic
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

        public async Task<bool> CreateEventAsync(EventDTO eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            // Due to model entity requiring location, user, category, create fake dummy data inside database

            var eventEntityMapped = _mapper.Map<eventplus.models.Entities.Event>(eventEntity);
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

        public async Task<List<EventDTO>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return _mapper.Map<List<EventDTO>>(events);
        }

        public async Task<EventDTO> GetEventByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var eventEntity = await _eventRepository.GetEventByIdAsync(id);
            return _mapper.Map<EventDTO>(eventEntity);
        }

        public async Task<bool> UpdateEventAsync(EventDTO eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            if (!ValidateEventData(eventEntity))
            {
                return false;
            }

            var eventEntityMapped = _mapper.Map<eventplus.models.Entities.Event>(eventEntity);
            return await _eventRepository.UpdateEventAsync(eventEntityMapped);
        }

        private static bool ValidateEventData(EventDTO eventDTO)
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
