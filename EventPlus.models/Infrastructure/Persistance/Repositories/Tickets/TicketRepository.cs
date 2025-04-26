using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories.Tickets
{
    public class TicketRepository : ITicketRepository
    {

        private readonly EventPlusContext _context;
        private readonly DbSet<Ticket> _tickets;

        public TicketRepository(EventPlusContext context)
        {
            _context = context;
            _tickets = context.Set<Ticket>();
        }

        public async Task<bool> CreateTicketAsync(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            ticket.ScannedDate = DateTime.MinValue;
            _tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _tickets.FindAsync(id);
            if (ticket == null)
            {
                return false;
            }
            _tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _tickets.Include(t => t.FkEventidEventNavigation)
                .Include(t => t.UserTicket)
                .Include(t => t.FkSeatingidSeatingNavigation)
                .Include(t => t.TypeNavigation)
                .ToListAsync();
        }
        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            var ticket = await _tickets
                .Include(t => t.FkEventidEventNavigation)
                .Include(t => t.UserTicket)
                .Include(t => t.FkSeatingidSeatingNavigation)
                .Include(t => t.TypeNavigation)
                .FirstOrDefaultAsync(t => t.IdTicket == id);
            return ticket;
        }
        public async Task<bool> UpdateTicketAsync(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            _tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
