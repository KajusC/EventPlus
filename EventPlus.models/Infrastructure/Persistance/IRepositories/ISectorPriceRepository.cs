using eventplus.models.Domain.Sectors;

namespace eventplus.models.Infrastructure.Persistance.IRepositories
{
	public interface ISectorPriceRepository
	{
		Task<bool> CreateSectorPriceAsync(SectorPrice sectorPrice);
		Task<bool> UpdateSectorPriceAsync(SectorPrice sectorPrice);
		Task<bool> DeleteSectorPriceAsync(int sectorId, int eventId);
		Task<SectorPrice?> GetSectorPriceByIdAsync(int sectorId, int eventId);
		Task<List<SectorPrice>> GetAllSectorPricesAsync();
		Task<List<SectorPrice>> GetSectorPricesByEventIdAsync(int eventId);
		Task<List<SectorPrice>> GetSectorPricesBySectorIdAsync(int sectorId);
		Task<bool> DeleteSectorPrices(int sectorId);
	}
}