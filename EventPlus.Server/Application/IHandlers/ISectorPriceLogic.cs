using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
	public interface ISectorPriceLogic
	{
		Task<bool> CreateSectorPriceAsync(SectorPriceViewModel sectorPriceEntity);
		Task<bool> UpdateSectorPriceAsync(SectorPriceViewModel sectorPriceEntity);
		Task<bool> DeleteSectorPriceAsync(int sectorId, int eventId);
		Task<SectorPriceViewModel> GetSectorPriceByIdAsync(int sectorId, int eventId);
		Task<List<SectorPriceViewModel>> GetAllSectorPricesAsync();
		Task<bool> DeleteSectorPrices(int sectorId);
	}
}