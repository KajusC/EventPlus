using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
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
		Task<List<Ticket>> GetTicketsByEventIdAsync(int eventId);
		Task<List<SectorPrice>> GetSectorPricesByEventIdAsync(int eventId);
		Task<List<Event>> GetSameCategoryEventSectorPricesAsync(int categoryId);
	}
}