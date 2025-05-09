using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventController : ControllerBase
	{
		private readonly IEventLogic _eventLogic;
		private readonly ITicketLogic _ticketLogic;
		private readonly IFeedbackLogic _feedbackLogic;
		private readonly ISectorLogic _sectorLogic;
		private readonly ISectorPriceLogic _sectorPriceLogic;
		private readonly ISeatingLogic _seatingLogic;

		public EventController(
			IEventLogic eventLogic,
			ITicketLogic ticketLogic,
			IFeedbackLogic feedbackLogic,
			ISectorLogic sectorLogic,
			ISectorPriceLogic sectorPriceLogic,
			ISeatingLogic seatingLogic)
		{
			_eventLogic = eventLogic;
			_ticketLogic = ticketLogic;
			_feedbackLogic = feedbackLogic;
			_sectorLogic = sectorLogic;
			_sectorPriceLogic = sectorPriceLogic;
			_seatingLogic = seatingLogic;
		}

		[HttpGet]
		public async Task<ActionResult<List<EventViewModel>>> GetAllEvents()
		{
			var events = await _eventLogic.GetAllEventsAsync();
			return Ok(events);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<EventViewModel>> GetEventById(int id)
		{
			var eventEntity = await _eventLogic.GetEventByIdAsync(id);
			if (eventEntity == null)
			{
				return NotFound();
			}
			return Ok(eventEntity);
		}

		[HttpPost]
		[Authorize(Roles = "Administrator, Organiser")]
		public async Task<ActionResult<bool>> CreateEvent([FromBody] EventViewModel eventEntity)
		{
			if (eventEntity == null)
			{
				return BadRequest("Event cannot be null");
			}
			var result = await _eventLogic.CreateEventAsync(eventEntity);
			return CreatedAtAction(nameof(GetEventById), new { id = eventEntity.IdEvent }, result);
		}

		[HttpPut]
		[Authorize(Roles = "Administrator, Organiser")]
		public async Task<ActionResult<bool>> UpdateEvent([FromBody] EventViewModel eventEntity)
		{
			if (eventEntity == null)
			{
				return BadRequest("Event cannot be null");
			}
			var result = await _eventLogic.UpdateEventAsync(eventEntity);
			return Ok(result);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Administrator")]
		public async Task<ActionResult<bool>> DeleteEvent(int id)
		{
			// Check if the event has tickets before deleting
			var hasTickets = await _ticketLogic.IfEventHasTickets(id);
			if (hasTickets)
			{
				return BadRequest("Cannot delete event with associated tickets.");
			}

			// If the event has feedback, delete them first
			var feedbackDeleted = await _feedbackLogic.DeleteEventFeedbacks(id);
			Console.WriteLine($"Feedback deleted: {feedbackDeleted}"); // should be logger

			// delete sectors
			var sectorsDeleted = await _sectorLogic.DeleteEventSectors(id);
			Console.WriteLine($"Sectors deleted: {sectorsDeleted}"); // should be logger

			var result = await _eventLogic.DeleteEventAsync(id);
			if (!result)
			{
				return NotFound();
			}
			return NoContent();
		}

		[HttpPost("CreateFullEvent")]
		[Authorize(Roles = "Administrator, Organiser")]
		public async Task<ActionResult<int>> CreateFullEvent([FromBody] CompleteEvent completeEventEntity)
		{
			if (completeEventEntity == null)
			{
				return BadRequest("Event cannot be null");
			}

			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState
						.Where(e => e.Value.Errors.Count > 0)
						.Select(e => new { Field = e.Key, Errors = e.Value.Errors.Select(er => er.ErrorMessage) })
						.ToList();

					return BadRequest(new { Message = "Validation failed", Errors = errors });
				}

				// Additional validation logic
				if (completeEventEntity.Event.StartDate > completeEventEntity.Event.EndDate)
				{
					return BadRequest("Start date cannot be later than end date");
				}

				var result = await _eventLogic.CreateFullEvent(
					completeEventEntity.Event,
					completeEventEntity.EventLocation,
					completeEventEntity.Partners,
					completeEventEntity.Performers,
					completeEventEntity.Sectors.Count > 0 ? completeEventEntity.Sectors[0] : new SectorViewModel(),
					completeEventEntity.Sectors,
					completeEventEntity.SectorPrices,
					completeEventEntity.Seatings);

				return CreatedAtAction(nameof(GetEventById), new { id = result }, result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating event: {ex.ToString()}");
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		public class CompleteEvent
		{
			public EventViewModel Event { get; set; }
			public EventLocationViewModel EventLocation { get; set; }
			public PartnerViewModel Partners { get; set; }
			public PerformerViewModel Performers { get; set; }
			public List<SectorViewModel> Sectors { get; set; } = new List<SectorViewModel>();
			public List<SectorPriceViewModel> SectorPrices { get; set; } = new List<SectorPriceViewModel>();
			public List<SeatingViewModel> Seatings { get; set; } = new List<SeatingViewModel>();
		}
	}
}