using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SectorPriceController : ControllerBase
	{
		private readonly ISectorPriceLogic _sectorPriceLogic;

		public SectorPriceController(
			ISectorPriceLogic sectorPriceLogic)
		{
			_sectorPriceLogic = sectorPriceLogic;
		}

		[HttpGet]
		public async Task<ActionResult<List<EventViewModel>>> GetAllSectorPrices()
		{
			var seatings = await _sectorPriceLogic.GetAllSectorPricesAsync();
			return Ok(seatings);
		}

		[HttpGet("{id},{eventId}")]
		public async Task<ActionResult<EventViewModel>> GetSectorPriceById(int id, int eventId)
		{
			var seatingEntity = await _sectorPriceLogic.GetSectorPriceByIdAsync(id, eventId);
			if (seatingEntity == null)
			{
				return NotFound();
			}
			return Ok(seatingEntity);
		}
	}
}