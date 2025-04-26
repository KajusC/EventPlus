using eventplus.models.Domain.Sectors;

namespace eventplus.models.Infrastructure.Persistance.Repositories.Sectors
{
    public interface ISectorRepository
    {
        Task<bool> CreateSectorAsync(Sector sector);
        Task<bool> UpdateSectorAsync(Sector sector);
        Task<bool> DeleteSectorAsync(int id);
        Task<Sector?> GetSectorByIdAsync(int id);
        Task<List<Sector>> GetAllSectorsAsync();
        Task<List<Sector>> GetAllSectorsByEventIdAsync(int eventId);
        Task<bool> DeleteEventSectors(int eventId);
    }
}
