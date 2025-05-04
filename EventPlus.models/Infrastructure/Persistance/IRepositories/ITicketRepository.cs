using eventplus.models.Domain.Tickets;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<bool> UpdateTicketScanDate(int ticketId);
    }
}
