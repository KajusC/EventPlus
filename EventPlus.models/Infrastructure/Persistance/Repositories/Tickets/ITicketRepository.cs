using eventplus.models.Domain.Tickets;

namespace eventplus.models.Infrastructure.Persistance.Repositories.Tickets
{
    public interface ITicketRepository
    {
        Task<bool> CreateTicketAsync(Ticket ticket);
        Task<bool> UpdateTicketAsync(Ticket ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<List<Ticket>> GetAllTicketsAsync();
    }
}
