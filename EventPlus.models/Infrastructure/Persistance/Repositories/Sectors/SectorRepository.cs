using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.context;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories.Sectors
{
    public class SectorRepository : ISectorRepository
    {

        private readonly EventPlusContext _context;
        private readonly DbSet<Sector> _sectors;
        private readonly DbSet<SectorPrice> _sectorPrices;

        public SectorRepository(EventPlusContext context)
        {
            _context = context;
            _sectors = context.Set<Sector>();
            _sectorPrices = context.Set<SectorPrice>();
        }

        public async Task<bool> CreateSectorAsync(Sector sector)
        {
            if (sector == null)
            {
                throw new ArgumentNullException(nameof(sector));
            }
            await _sectors.AddAsync(sector);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEventSectors(int eventId)
        {
            var sectors = await _sectors.Where(s => s.FkEventLocationidEventLocation == eventId).ToListAsync();
            if (sectors.Count == 0)
            {
                return false;
            }
            _sectors.RemoveRange(sectors);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSectorAsync(int id)
        {
            // We need to get both parts of the composite key
            var sector = await _sectors
                .FirstOrDefaultAsync(s => s.IdSector == id);

            if (sector == null)
            {
                return false;
            }

            // Use the complete composite key to identify the sector
            var sectorToDelete = await _sectors
                .FirstOrDefaultAsync(s => s.IdSector == sector.IdSector &&
                                         s.FkEventLocationidEventLocation == sector.FkEventLocationidEventLocation);

            if (sectorToDelete == null)
            {
                return false;
            }

            // Delete related sector prices first (even though cascade should handle this)
            var sectorPrices = await _sectorPrices
                .Where(sp => sp.FkSectoridSector == sector.IdSector &&
                            sp.FkEventidEvent == sector.FkEventLocationidEventLocation)
                .ToListAsync();

            if (sectorPrices.Any())
            {
                _sectorPrices.RemoveRange(sectorPrices);
                await _context.SaveChangesAsync();
            }

            // Now delete the sector itself
            _sectors.Remove(sectorToDelete);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<Sector>> GetAllSectorsAsync()
        {
            return await _sectors
                .Include(s => s.FkEventLocationidEventLocationNavigation)
                .Include(s => s.SectorPrices)
                .Include(s => s.Seatings)
                .ToListAsync();
        }

        public async Task<List<Sector>> GetAllSectorsByEventIdAsync(int eventId)
        {
            return await _sectors
                .Include(s => s.FkEventLocationidEventLocationNavigation)
                .Include(s => s.SectorPrices)
                .Include(s => s.Seatings)
                .Where(s => s.FkEventLocationidEventLocation == eventId)
                .ToListAsync();
        }

        public async Task<Sector?> GetSectorByIdAsync(int id)
        {
            return await _sectors
                .Include(s => s.FkEventLocationidEventLocationNavigation)
                .Include(s => s.SectorPrices)
                .Include(s => s.Seatings)
                .FirstOrDefaultAsync(s => s.IdSector == id);
        }

        public async Task<bool> UpdateSectorAsync(Sector sector)
        {
            if (sector == null)
            {
                throw new ArgumentNullException(nameof(sector));
            }
            _sectors.Update(sector);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<bool> DeleteSectorPricesAsync(int sectorId)
        {
            var sectorPrices = await _sectorPrices
                .Include(sp => sp.FkEventidEventNavigation)
                .Include(sp => sp.FkSectoridSector)
                .Where(sp => sp.FkSectoridSector == sectorId)
                .ToListAsync();
            if (sectorPrices.Count == 0)
            {
                return false;
            }
            _sectorPrices.RemoveRange(sectorPrices);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
