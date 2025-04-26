using AutoMapper;
using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.Persistance.Repositories.Tickets;
using EventPlus.Server.Application.Tickets.ViewModel;

namespace EventPlus.Server.Application.Tickets.Handler
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

        public async Task<bool> CreateTicketAsync(TicketViewModel ticket)
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

        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return _mapper.Map<List<TicketViewModel>>(tickets);
        }

        public async Task<TicketViewModel> GetTicketByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var ticketEntity = await _ticketRepository.GetTicketByIdAsync(id);
            return _mapper.Map<TicketViewModel>(ticketEntity);
        }

        public async Task<bool> UpdateTicketAsync(TicketViewModel ticket)
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