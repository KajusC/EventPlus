using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SeatingController : ControllerBase
	{
		private readonly ISectorLogic _sectorLogic;
		private readonly ISectorPriceLogic _sectorPriceLogic;
		private readonly ISeatingLogic _seatingLogic;

		public SeatingController(
			ISectorLogic sectorLogic,
			ISectorPriceLogic sectorPriceLogic,
			ISeatingLogic seatingLogic)
		{
			_sectorLogic = sectorLogic;
			_sectorPriceLogic = sectorPriceLogic;
			_seatingLogic = seatingLogic;
		}

		[HttpGet]
		public async Task<ActionResult<List<EventViewModel>>> GetAllSeatings()
		{
			var seatings = await _seatingLogic.GetAllSeatingsAsync();
			return Ok(seatings);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<EventViewModel>> GetSeatingById(int id)
		{
			var seatingEntity = await _seatingLogic.GetSeatingByIdAsync(id);
			if (seatingEntity == null)
			{
				return NotFound();
			}
			return Ok(seatingEntity);
		}
	}
}