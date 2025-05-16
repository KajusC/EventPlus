using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SectorController : ControllerBase
	{
		private readonly ISectorLogic _sectorLogic;

		public SectorController(
			ISectorLogic sectorLogic)
		{
			_sectorLogic = sectorLogic;
		}

		[HttpGet]
		public async Task<ActionResult<List<EventViewModel>>> GetAllSectors()
		{
			var seatings = await _sectorLogic.GetAllSectorsAsync();
			return Ok(seatings);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<EventViewModel>> GetSectorById(int id)
		{
			var seatingEntity = await _sectorLogic.GetSectorByIdAsync(id);
			if (seatingEntity == null)
			{
				return NotFound();
			}
			return Ok(seatingEntity);
		}
	}
}