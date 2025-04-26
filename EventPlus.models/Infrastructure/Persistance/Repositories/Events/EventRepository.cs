using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories.Events
{
    public class EventRepository : IEventRepository
    {

        private readonly EventPlusContext _context;
        private readonly DbSet<Event> _events;

        public EventRepository(EventPlusContext context)
        {
            _context = context;
            _events = context.Set<Event>();
        }

        public async Task<bool> CreateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }
            await _events.AddAsync(eventEntity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventEntity = await _events.FindAsync(id);
            if (eventEntity == null)
            {
                return false;
            }
            _events.Remove(eventEntity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _events
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            var eventEntity = await _events
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
            var events = await _events
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
            var events = await _events
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
            var events = await _events
                .Include(e => e.CategoryNavigation)
                .Include(e => e.FkEventLocationidEventLocationNavigation)
                .Include(e => e.FkOrganiseridUserNavigation)
                .Include(e => e.SectorPrices)
                .Include(e => e.Tickets)
                .Where(e => e.FkOrganiseridUser == userId)
                .ToListAsync();
            return events;
        }

        public async Task<bool> UpdateEventAsync(Event eventEntity)
        {
            if (eventEntity == null)
            {
                throw new ArgumentNullException(nameof(eventEntity));
            }

            var existingEvent = await _events.AsNoTracking().AnyAsync(e => e.IdEvent == eventEntity.IdEvent);
            if (!existingEvent)
            {
                return false;
            }


            _events.Update(eventEntity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
