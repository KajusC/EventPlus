using EventPlus.Server.DTO;

namespace EventPlus.Server.Logic.Interface
{
    public interface ISectorLogic
    {
        Task<List<SectorDTO>> GetAllSectorsAsync();
        Task<SectorDTO> GetSectorByIdAsync(int id);
        Task<bool> CreateSectorAsync(SectorDTO sectorEntity);
        Task<bool> UpdateSectorAsync(SectorDTO sectorEntity);
        Task<bool> DeleteSectorAsync(int id);

        Task<bool> DeleteEventSectors(int eventId);
    }
}
