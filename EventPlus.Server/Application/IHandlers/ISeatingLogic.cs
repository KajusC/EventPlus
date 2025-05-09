using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
	public interface ISeatingLogic
	{
		Task<List<SeatingViewModel>> GetAllSeatingsAsync();
		Task<SeatingViewModel> GetSeatingByIdAsync(int id);
		Task<bool> CreateSeatingAsync(SeatingViewModel seatingEntity);
		Task<bool> UpdateSeatingAsync(SeatingViewModel seatingEntity);
		Task<bool> DeleteSeatingAsync(int id);
		Task<bool> DeleteSeatings(int sectorId);
		Task<List<SeatingViewModel>> GetSeatingsForSectorAsync(int sectorId);
	}
}