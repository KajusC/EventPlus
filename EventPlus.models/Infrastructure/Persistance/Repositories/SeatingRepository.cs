using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.context;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.Infrastructure.Persistance.Repositories
{
	public class SeatingRepository : ISeatingRepository
	{
		private readonly EventPlusContext _context;
		private readonly DbSet<Seating> _seatings;

		public SeatingRepository(EventPlusContext context)
		{
			_context = context;
			_seatings = context.Set<Seating>();
		}

		public async Task<bool> CreateSeatingAsync(Seating seating)
		{
			if (seating == null)
			{
				throw new ArgumentNullException(nameof(seating));
			}
			await _seatings.AddAsync(seating);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteSeatingAsync(int id)
		{
			var seating = await _seatings.FindAsync(id);
			if (seating == null)
			{
				return false;
			}
			_seatings.Remove(seating);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<List<Seating>> GetAllSeatingsAsync()
		{
			return await _seatings
				.Include(s => s.FkSectoridSectorNavigation)
				.ToListAsync();
		}

		public async Task<Seating?> GetSeatingByIdAsync(int id)
		{
			return await _seatings
				.Include(s => s.FkSectoridSectorNavigation)
				.FirstOrDefaultAsync(s => s.IdSeating == id);
		}

		public async Task<List<Seating>> GetSeatingsBySectorIdAsync(int sectorId)
		{
			return await _seatings
				.Include(s => s.FkSectoridSectorNavigation)
				.Where(s => s.FkSectoridSector == sectorId)
				.ToListAsync();
		}

		public async Task<bool> UpdateSeatingAsync(Seating seating)
		{
			if (seating == null)
			{
				throw new ArgumentNullException(nameof(seating));
			}
			_seatings.Update(seating);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}