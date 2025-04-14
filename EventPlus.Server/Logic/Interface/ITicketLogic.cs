using eventplus.models.Entities;
using EventPlus.Server.DTO;

namespace EventPlus.Server.Logic.Interface
{
    public interface ITicketLogic
    {
        Task<bool> CreateTicketAsync(TicketDTO ticket);
        Task<bool> UpdateTicketAsync(TicketDTO ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<TicketDTO> GetTicketByIdAsync(int id);
        Task<List<TicketDTO>> GetAllTicketsAsync();
        Task<bool> IfEventHasTickets(int eventId);
    }
}
