using EventPlus.Server.Application.Tickets.ViewModel;

namespace EventPlus.Server.Application.Tickets.Handler
{
    public interface ITicketLogic
    {
        Task<bool> CreateTicketAsync(TicketViewModel ticket);
        Task<bool> UpdateTicketAsync(TicketViewModel ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<TicketViewModel> GetTicketByIdAsync(int id);
        Task<List<TicketViewModel>> GetAllTicketsAsync();
        Task<bool> IfEventHasTickets(int eventId);
    }
}
