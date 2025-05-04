using eventplus.models.Domain.Events;
using eventplus.models.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
    public class EventLocationRepository : Repository<EventLocation>
    {
        public EventLocationRepository(EventPlusContext context) : base(context)
        {
        }


        public override async Task<List<EventLocation>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Event)
                .Include(e => e.HoldingEquipmentNavigation)
                .Include(e => e.Sectors)
                .ToListAsync();
        }

        public override async Task<EventLocation> GetByIdAsync(int id)
        {
            var eventLocation = await _dbSet
                .Include(e => e.Event)
                .Include(e => e.HoldingEquipmentNavigation)
                .Include(e => e.Sectors)
                .FirstOrDefaultAsync(e => e.IdEventLocation == id);
            if (eventLocation == null)
            {
                throw new KeyNotFoundException($"EventLocation with ID {id} not found.");
            }
            return eventLocation;
        }
    }
}
