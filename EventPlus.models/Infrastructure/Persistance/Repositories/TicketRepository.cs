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
            ticket.ScannedDate = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Unspecified);
            _dbSet.Add(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        public override async Task<List<Ticket>> GetAllAsync()
        {
            return await _dbSet.Include(t => t.FkEventidEventNavigation)
                .Include(t => t.UserTicket)
                .Include(t => t.FkSeatingidSeatingNavigation)
                .Include(t => t.FkTicketstatusNavigation)
                .Include(t => t.TypeNavigation)
                .ToListAsync();
        }
        public override async Task<Ticket> GetByIdAsync(int id)
        {
            var ticket = await _dbSet
                .Include(t => t.FkEventidEventNavigation)
                .Include(t => t.UserTicket)
                .Include(t => t.FkSeatingidSeatingNavigation)
                .Include(t => t.FkTicketstatusNavigation)
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
            ticket.ScannedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<int> GetIdByQrCode(string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                throw new ArgumentNullException(nameof(qrCode), "QR code cannot be null or empty.");
            }
            var ticket = _dbSet.FirstOrDefault(t => t.QrCode == qrCode);
            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }
            return Task.FromResult(ticket.IdTicket);
        }

        public async Task<bool> UpdateTicketStatusInvalid(int ticketId)
        {
            var ticket = await _dbSet.FindAsync(ticketId);
            if (ticket == null)
            {
                return false;
            }

            var ticketStatus = await _context.Ticketstatuses.FirstOrDefaultAsync(ts => ts.Name == "Invalid");

            if (ticketStatus == null)
            {
                throw new KeyNotFoundException("Ticket status of \"Invalid\" not found.");
            }

            ticket.FkTicketstatus = ticketStatus.IdStatus;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTicketStatusRead(int ticketId)
        {
            var ticket = await _dbSet.FindAsync(ticketId);
            if (ticket == null)
            {
                return false;
            }
            var ticketStatus = await _context.Ticketstatuses.FirstOrDefaultAsync(ts => ts.Name == "Scanned");
            if (ticketStatus == null)
            {
                throw new KeyNotFoundException("Ticket status of \"Scanned\" not found.");
            }

            ticket.FkTicketstatus = ticketStatus.IdStatus;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
