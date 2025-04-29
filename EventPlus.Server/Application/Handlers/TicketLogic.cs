using AutoMapper;
using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
    public class TicketLogic : ITicketLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketLogic(
            IUnitOfWork unitOfWork,
            ITicketRepository ticketRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateTicketAsync(TicketViewModel ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var ticketEntity = _mapper.Map<Ticket>(ticket);
            return await _unitOfWork.Tickets.CreateAsync(ticketEntity);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _unitOfWork.Tickets.DeleteAsync(id);
        }

        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return _mapper.Map<List<TicketViewModel>>(tickets);
        }

        public async Task<TicketViewModel> GetTicketByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var ticketEntity = await _unitOfWork.Tickets.GetByIdAsync(id);
            return _mapper.Map<TicketViewModel>(ticketEntity);
        }

        public async Task<bool> UpdateTicketAsync(TicketViewModel ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var ticketEntity = _mapper.Map<Ticket>(ticket);
            return await _unitOfWork.Tickets.UpdateAsync(ticketEntity);
        }

        public async Task<bool> IfEventHasTickets(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return tickets.Any(t => t.FkEventidEvent == eventId);
        }
    }
}