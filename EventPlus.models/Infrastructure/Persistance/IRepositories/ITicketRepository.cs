using eventplus.models.Domain.Tickets;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<bool> UpdateTicketScanDate(int ticketId);
        Task<int> GetIdByQrCode(string qrCode);
        Task<bool> UpdateTicketStatusInvalid(int ticketId);
        Task<bool> UpdateTicketStatusRead(int ticketId);
        Task<List<Ticket>> GetTicketsByUserIdAsync(int userId);
    }
}