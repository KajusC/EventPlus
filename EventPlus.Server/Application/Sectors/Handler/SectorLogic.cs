using AutoMapper;
using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.Persistance.Repositories.Sectors;
using EventPlus.Server.Application.Sectors.ViewModel;

namespace EventPlus.Server.Application.Sectors.Handler
{
    public class SectorLogic : ISectorLogic
    {

        private readonly ISectorRepository _sectorRepository;
        private readonly IMapper _mapper;

        public SectorLogic(ISectorRepository sectorRepository, IMapper mapper)
        {
            _sectorRepository = sectorRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateSectorAsync(SectorViewModel sectorEntity)
        {
            if (sectorEntity == null)
            {
                throw new ArgumentNullException(nameof(sectorEntity));
            }
            var sector = _mapper.Map<Sector>(sectorEntity);
            return await _sectorRepository.CreateSectorAsync(sector);
        }

        public Task<bool> DeleteEventSectors(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }
            return _sectorRepository.DeleteEventSectors(eventId);
        }

        public async Task<bool> DeleteSectorAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _sectorRepository.DeleteSectorAsync(id);
        }

        public async Task<List<SectorViewModel>> GetAllSectorsAsync()
        {
            var sectors = await _sectorRepository.GetAllSectorsAsync();
            return _mapper.Map<List<SectorViewModel>>(sectors);
        }

        public async Task<SectorViewModel> GetSectorByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var sectorEntity = await _sectorRepository.GetSectorByIdAsync(id);
            return _mapper.Map<SectorViewModel>(sectorEntity);
        }

        public async Task<bool> UpdateSectorAsync(SectorViewModel sectorEntity)
        {
            if (sectorEntity == null)
            {
                throw new ArgumentNullException(nameof(sectorEntity));
            }
            var sector = _mapper.Map<Sector>(sectorEntity);
            return await _sectorRepository.UpdateSectorAsync(sector);
        }
    }
}
