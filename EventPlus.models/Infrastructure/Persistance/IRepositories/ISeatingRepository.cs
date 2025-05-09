using eventplus.models.Domain.Sectors;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
	public interface ISeatingRepository
	{
		Task<bool> CreateSeatingAsync(Seating seating);
		Task<bool> UpdateSeatingAsync(Seating seating);
		Task<bool> DeleteSeatingAsync(int id);
		Task<Seating?> GetSeatingByIdAsync(int id);
		Task<List<Seating>> GetAllSeatingsAsync();
		Task<List<Seating>> GetSeatingsBySectorIdAsync(int sectorId);
	}
}