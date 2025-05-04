using AutoMapper;
using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
    public class SectorLogic : ISectorLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SectorLogic(IUnitOfWork unitOfWork,
            ISectorRepository sectorRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateSectorAsync(SectorViewModel sectorEntity)
        {
            if (sectorEntity == null)
            {
                throw new ArgumentNullException(nameof(sectorEntity));
            }
            var sector = _mapper.Map<Sector>(sectorEntity);
            return await _unitOfWork.Sectors.CreateSectorAsync(sector);
        }

        public Task<bool> DeleteEventSectors(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }
            return _unitOfWork.Sectors.DeleteEventSectors(eventId);
        }

        public async Task<bool> DeleteSectorAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _unitOfWork.Sectors.DeleteSectorAsync(id);
        }

        public async Task<List<SectorViewModel>> GetAllSectorsAsync()
        {
            var sectors = await _unitOfWork.Sectors.GetAllSectorsAsync();
            return _mapper.Map<List<SectorViewModel>>(sectors);
        }

        public async Task<SectorViewModel> GetSectorByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var sectorEntity = await _unitOfWork.Sectors.GetSectorByIdAsync(id);
            return _mapper.Map<SectorViewModel>(sectorEntity);
        }

        public async Task<bool> UpdateSectorAsync(SectorViewModel sectorEntity)
        {
            if (sectorEntity == null)
            {
                throw new ArgumentNullException(nameof(sectorEntity));
            }
            var sector = _mapper.Map<Sector>(sectorEntity);
            return await _unitOfWork.Sectors.UpdateSectorAsync(sector);
        }
    }
}
