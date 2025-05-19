using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(EventPlusContext context) : base(context)
        {
        }

        public override async Task<List<Event>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .ToListAsync();
        }

        public override async Task<Event> GetByIdAsync(int id)
        {
            var eventEntity = await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.IdEvent == id);
            if (eventEntity == null)
            {
                throw new KeyNotFoundException($"Event with ID {id} not found.");
            }
            return eventEntity;
        }

        public async Task<List<Event>> GetEventsByCategoryIdAsync(int categoryId)
        {
            var events = await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .Where(e => e.Category == categoryId)
                .ToListAsync();
            return events;
        }

        public async Task<List<Event>> GetEventsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            var events = await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .Where(e => e.StartDate >= startDate && e.EndDate <= endDate)
                .ToListAsync();
            return events;
        }

        public async Task<List<Event>> GetEventsByUserIdAsync(int userId)
        {
            var events = await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .Where(e => e.FkOrganiseridUser == userId)
                .ToListAsync();
            return events;
        }

        public override async Task<bool> UpdateAsync(Event eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            var existingEvent = await _dbSet.AsNoTracking().AnyAsync(e => e.IdEvent == eventEntity.IdEvent);
            if (!existingEvent)
            {
                return false;
            }


            _dbSet.Update(eventEntity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Event>> GetEventsByUserTicketsAsync(int userId)
        {
            var events = await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .ThenInclude(t => t.UserTicket)
                .ThenInclude(ut => ut.FkUseridUserNavigation)
                .Where(e => e.Tickets.Any(t => t.UserTicket.FkUseridUser == userId))
                .ToListAsync();
            return events;
        }

        public async Task<List<Event>> GetEventsByOrganiserIdAsync(int organiserId)
        {
            var events = await _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .Where(e => e.FkOrganiseridUser == organiserId)
                .ToListAsync();
            return events;
        }

        public Task<List<Event>> GetEventsByLocationIdAsync(int locationId)
        {
            var events = _dbSet
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .Where(e => e.FkEventLocationidEventLocation == locationId);
            return events.ToListAsync();
        }
    }
}
