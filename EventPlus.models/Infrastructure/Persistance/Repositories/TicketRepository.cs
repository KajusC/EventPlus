using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {

        public TicketRepository(EventPlusContext context) : base(context)
        {
        }

        public override async Task<bool> CreateAsync(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            ticket.ScannedDate = DateTime.MinValue;
            _dbSet.Add(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        public override async Task<List<Ticket>> GetAllAsync()
        {
            return await _dbSet.Include(t => t.FkEventidEventNavigation)
                .Include(t => t.UserTicket)
                .Include(t => t.FkSeatingidSeatingNavigation)
                .Include(t => t.TypeNavigation)
                .ToListAsync();
        }
        public override async Task<Ticket> GetByIdAsync(int id)
        {
            var ticket = await _dbSet
                .Include(t => t.FkEventidEventNavigation)
                .Include(t => t.UserTicket)
                .Include(t => t.FkSeatingidSeatingNavigation)
                .Include(t => t.TypeNavigation)
                .FirstOrDefaultAsync(t => t.IdTicket == id);
            return ticket;
        }

        public async Task<bool> UpdateTicketScanDate(int ticketId)
        {
            var ticket = await _dbSet.FindAsync(ticketId);
            if (ticket == null)
            {
                return false;
            }
            ticket.ScannedDate = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
