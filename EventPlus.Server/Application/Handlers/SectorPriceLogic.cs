using AutoMapper;
using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
	public class SectorPriceLogic : ISectorPriceLogic
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public SectorPriceLogic(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<bool> CreateSectorPriceAsync(SectorPriceViewModel sectorPriceEntity)
		{
			if (sectorPriceEntity == null)
			{
				throw new ArgumentNullException(nameof(sectorPriceEntity));
			}
			var sectorPrice = _mapper.Map<SectorPrice>(sectorPriceEntity);
			return await _unitOfWork.SectorPrices.CreateSectorPriceAsync(sectorPrice);
		}

		public async Task<bool> DeleteSectorPriceAsync(int sectorId, int eventId)
		{
			if (sectorId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sectorId), "Sector ID must be greater than zero.");
			}
			if (eventId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
			}
			return await _unitOfWork.SectorPrices.DeleteSectorPriceAsync(sectorId, eventId);
		}

		public Task<bool> DeleteSectorPrices(int sectorId)
		{
			if (sectorId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sectorId), "Sector ID must be greater than zero.");
			}
			return _unitOfWork.SectorPrices.DeleteSectorPrices(sectorId);
		}

		public async Task<List<SectorPriceViewModel>> GetAllSectorPricesAsync()
		{
			var sectorPrices = await _unitOfWork.SectorPrices.GetAllSectorPricesAsync();
			return _mapper.Map<List<SectorPriceViewModel>>(sectorPrices);
		}

		public async Task<SectorPriceViewModel> GetSectorPriceByIdAsync(int sectorId, int eventId)
		{
			if (sectorId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sectorId), "Sector ID must be greater than zero.");
			}
			if (eventId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
			}
			var sectorPriceEntity = await _unitOfWork.SectorPrices.GetSectorPriceByIdAsync(sectorId, eventId);
			return _mapper.Map<SectorPriceViewModel>(sectorPriceEntity);
		}

		public async Task<bool> UpdateSectorPriceAsync(SectorPriceViewModel sectorPriceEntity)
		{
			if (sectorPriceEntity == null)
			{
				throw new ArgumentNullException(nameof(sectorPriceEntity));
			}
			var sectorPrice = _mapper.Map<SectorPrice>(sectorPriceEntity);
			return await _unitOfWork.SectorPrices.UpdateSectorPriceAsync(sectorPrice);
		}
	}
}