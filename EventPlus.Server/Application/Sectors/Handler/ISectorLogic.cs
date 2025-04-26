using EventPlus.Server.Application.Sectors.ViewModel;

namespace EventPlus.Server.Application.Sectors.Handler
{
    public interface ISectorLogic
    {
        Task<List<SectorViewModel>> GetAllSectorsAsync();
        Task<SectorViewModel> GetSectorByIdAsync(int id);
        Task<bool> CreateSectorAsync(SectorViewModel sectorEntity);
        Task<bool> UpdateSectorAsync(SectorViewModel sectorEntity);
        Task<bool> DeleteSectorAsync(int id);

        Task<bool> DeleteEventSectors(int eventId);
    }
}
