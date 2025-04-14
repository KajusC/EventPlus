using AutoMapper;
using eventplus.models.Entities;
using eventplus.models.Repository.TicketRepository;
using EventPlus.Server.DTO;
using EventPlus.Server.Logic.Interface;

namespace EventPlus.Server.Logic
{
    public class TicketLogic : ITicketLogic
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketLogic(ITicketRepository ticketRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateTicketAsync(TicketDTO ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var ticketEntity = _mapper.Map<Ticket>(ticket);
            return await _ticketRepository.CreateTicketAsync(ticketEntity);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _ticketRepository.DeleteTicketAsync(id);
        }

        public async Task<List<TicketDTO>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return _mapper.Map<List<TicketDTO>>(tickets);
        }

        public async Task<TicketDTO> GetTicketByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var ticketEntity = await _ticketRepository.GetTicketByIdAsync(id);
            return _mapper.Map<TicketDTO>(ticketEntity);
        }

        public async Task<bool> UpdateTicketAsync(TicketDTO ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var ticketEntity = _mapper.Map<Ticket>(ticket);
            return await _ticketRepository.UpdateTicketAsync(ticketEntity);
        }

        public async Task<bool> IfEventHasTickets(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return tickets.Any(t => t.FkEventidEvent == eventId);
        }
    }
}