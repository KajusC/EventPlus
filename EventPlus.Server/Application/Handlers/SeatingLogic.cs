using AutoMapper;
using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
	public class SeatingLogic : ISeatingLogic
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public SeatingLogic(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<bool> CreateSeatingAsync(SeatingViewModel seatingEntity)
		{
			if (seatingEntity == null)
			{
				throw new ArgumentNullException(nameof(seatingEntity));
			}
			var seating = _mapper.Map<Seating>(seatingEntity);
			return await _unitOfWork.Seatings.CreateSeatingAsync(seating);
		}

		public async Task<bool> DeleteSeatingAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
			}
			return await _unitOfWork.Seatings.DeleteSeatingAsync(id);
		}

		public Task<bool> DeleteSeatings(int sectorId)
		{
			if (sectorId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sectorId), "Sector ID must be greater than zero.");
			}
			return _unitOfWork.Seatings.DeleteSeatingAsync(sectorId);
		}

		public async Task<List<SeatingViewModel>> GetAllSeatingsAsync()
		{
			var seatings = await _unitOfWork.Seatings.GetAllSeatingsAsync();
			return _mapper.Map<List<SeatingViewModel>>(seatings);
		}

		public async Task<SeatingViewModel> GetSeatingByIdAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
			}
			var seatingEntity = await _unitOfWork.Seatings.GetSeatingByIdAsync(id);
			return _mapper.Map<SeatingViewModel>(seatingEntity);
		}

		public async Task<List<SeatingViewModel>> GetSeatingsForSectorAsync(int sectorId)
		{
			if (sectorId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sectorId), "Sector ID must be greater than zero.");
			}
			var seatings = await _unitOfWork.Seatings.GetSeatingsBySectorIdAsync(sectorId);
			return _mapper.Map<List<SeatingViewModel>>(seatings);
		}

		public async Task<bool> UpdateSeatingAsync(SeatingViewModel seatingEntity)
		{
			if (seatingEntity == null)
			{
				throw new ArgumentNullException(nameof(seatingEntity));
			}
			var seating = _mapper.Map<Seating>(seatingEntity);
			return await _unitOfWork.Seatings.UpdateSeatingAsync(seating);
		}
	}
}