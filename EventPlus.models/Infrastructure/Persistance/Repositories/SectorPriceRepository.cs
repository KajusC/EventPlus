using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
	public class SectorPriceRepository : ISectorPriceRepository
	{
		private readonly EventPlusContext _context;
		private readonly DbSet<SectorPrice> _sectorPrices;

		public SectorPriceRepository(EventPlusContext context)
		{
			_context = context;
			_sectorPrices = context.Set<SectorPrice>();
		}

		public async Task<bool> CreateSectorPriceAsync(SectorPrice sectorPrice)
		{
			if (sectorPrice == null)
			{
				throw new ArgumentNullException(nameof(sectorPrice));
			}
			await _sectorPrices.AddAsync(sectorPrice);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteSectorPriceAsync(int sectorId, int eventId)
		{
			var sectorPrice = await _sectorPrices
				.FirstOrDefaultAsync(sp => sp.FkSectoridSector == sectorId && sp.FkEventidEvent == eventId);

			if (sectorPrice == null)
			{
				return false;
			}

			_sectorPrices.Remove(sectorPrice);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteSectorPrices(int sectorId)
		{
			var sectorPrices = await _sectorPrices
				.Where(sp => sp.FkSectoridSector == sectorId)
				.ToListAsync();

			if (!sectorPrices.Any())
			{
				return false;
			}

			_sectorPrices.RemoveRange(sectorPrices);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<SectorPrice>> GetAllSectorPricesAsync()
		{
			return await _sectorPrices
				.Include(sp => sp.FkSectoridSectorNavigation)
				.Include(sp => sp.FkEventidEventNavigation)
				.ToListAsync();
		}

		public async Task<SectorPrice?> GetSectorPriceByIdAsync(int sectorId, int eventId)
		{
			return await _sectorPrices
				.Include(sp => sp.FkSectoridSectorNavigation)
				.Include(sp => sp.FkEventidEventNavigation)
				.FirstOrDefaultAsync(sp => sp.FkSectoridSector == sectorId && sp.FkEventidEvent == eventId);
		}

		public async Task<List<SectorPrice>> GetSectorPricesByEventIdAsync(int eventId)
		{
			return await _sectorPrices
				.Include(sp => sp.FkSectoridSectorNavigation)
				.Include(sp => sp.FkEventidEventNavigation)
				.Where(sp => sp.FkEventidEvent == eventId)
				.ToListAsync();
		}

		public async Task<List<SectorPrice>> GetSectorPricesBySectorIdAsync(int sectorId)
		{
			return await _sectorPrices
				.Include(sp => sp.FkSectoridSectorNavigation)
				.Include(sp => sp.FkEventidEventNavigation)
				.Where(sp => sp.FkSectoridSector == sectorId)
				.ToListAsync();
		}

		public async Task<bool> UpdateSectorPriceAsync(SectorPrice sectorPrice)
		{
			if (sectorPrice == null)
			{
				throw new ArgumentNullException(nameof(sectorPrice));
			}
			_sectorPrices.Update(sectorPrice);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}