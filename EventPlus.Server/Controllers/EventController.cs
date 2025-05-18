using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
		private readonly IOrganiserLogic _organiserLogic;

		public EventController(
			IEventLogic eventLogic,
			ITicketLogic ticketLogic,
			IFeedbackLogic feedbackLogic,
			ISectorLogic sectorLogic,
			ISectorPriceLogic sectorPriceLogic,
			ISeatingLogic seatingLogic,
			IOrganiserLogic organiserLogic)
		{
			_eventLogic = eventLogic;
			_ticketLogic = ticketLogic;
			_feedbackLogic = feedbackLogic;
			_sectorLogic = sectorLogic;
			_sectorPriceLogic = sectorPriceLogic;
			_seatingLogic = seatingLogic;
			_organiserLogic = organiserLogic;
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
			var hasTickets = await _ticketLogic.IfEventHasTickets(id);
			if (hasTickets)
			{
				return BadRequest("Cannot delete event with associated tickets.");
			}

			var feedbackDeleted = await _feedbackLogic.DeleteEventFeedbacks(id);
			Console.WriteLine($"Feedback deleted: {feedbackDeleted}");

			var sectorsDeleted = await _sectorLogic.DeleteEventSectors(id);
			Console.WriteLine($"Sectors deleted: {sectorsDeleted}");

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

		[HttpGet("toprated")]
		[AllowAnonymous]
		public async Task<ActionResult<List<EventViewModel>>> InitializeEventRecomm([FromQuery] int userId)
		{
			var hashMap = new Dictionary<int, EventViewModel>();
			var weights = new Dictionary<int, RecommendedWeight>();

			// Sequential fetch of initial data since they use DbContext
			var visitedEvents = await GetVisitedEvents(userId);
			var tickets = await FetchUserTickets(userId);
			var allEvents = await _eventLogic.GetAllEventsAsync();
			
			// Pre-fetch all feedbacks to avoid concurrent DbContext access
			var allFeedbacks = await _feedbackLogic.GetAllFeedbacksAsync();
			
			// Initialize weights
			foreach (var evt in allEvents)
			{
				weights[evt.IdEvent] = new RecommendedWeight();
			}

			// Process events in parallel batches
			var eventTasks = new List<Task>();
			var batchSize = 5;
			
			for (int i = 0; i < allEvents.Count; i += batchSize)
			{
				var batch = allEvents.Skip(i).Take(batchSize);
				var batchTasks = batch.Select(evt =>
				{
					return Task.Run(() =>
					{
						// Get feedback for current event from pre-fetched feedbacks
						var eventFeedbacks = allFeedbacks.Where(f => f.FkEventidEvent == evt.IdEvent).ToList();

						if (eventFeedbacks.Any())
						{
							var feedbackAverage = eventFeedbacks.Average(f => f.Rating ?? 0);
							lock (weights)
							{
								if (feedbackAverage > 3)
								{
									weights[evt.IdEvent].IncreaseWeight();
								}
								else if (feedbackAverage < 3)
								{
									weights[evt.IdEvent].DecreaseWeight();
								}
							}
						}

						// Calculate organiser rating
						var organiserEvents = visitedEvents.Where(ve => ve.FkOrganiseridUser == evt.FkOrganiseridUser);
						if (organiserEvents.Any())
						{
							var organiserEventFeedbacks = allFeedbacks
								.Where(f => f.FkEventidEvent == organiserEvents.First().IdEvent)
								.ToList();

							if (organiserEventFeedbacks.Any())
							{
								var avgRating = organiserEventFeedbacks.Average(f => f.Rating ?? 0);
								lock (weights)
								{
									if (avgRating > 3)
									{
										weights[evt.IdEvent].IncreaseWeight();
									}
									else if (avgRating < 3)
									{
										weights[evt.IdEvent].DecreaseWeight();
									}
								}
							}
						}

						lock (hashMap)
						{
							hashMap[evt.IdEvent] = evt;
						}
					});
				});

				eventTasks.AddRange(batchTasks);
				
				// Wait for the current batch to complete
				if (eventTasks.Count >= batchSize)
				{
					await Task.WhenAll(eventTasks);
					eventTasks.Clear();
				}
			}

			// Wait for any remaining tasks
			if (eventTasks.Any())
			{
				await Task.WhenAll(eventTasks);
			}

			// Generate recommendations
			var recommendedEvents = allEvents
				.Where(e => !visitedEvents.Any(ve => ve.IdEvent == e.IdEvent))
				.OrderByDescending(e => weights[e.IdEvent].Weight)
				.Take(10)
				.ToList();

			return Ok(recommendedEvents);
		}

		private async Task<List<EventViewModel>> GetVisitedEvents(int userId)
		{
			var visitedEvents = await _eventLogic.GetEventsByUserTicketsAsync(userId);
			return visitedEvents;
		}

		private async Task<List<TicketViewModel>> FetchUserTickets(int userId)
		{
			var boughtTickets = await _ticketLogic.GetTicketsByUserIdAsync(userId);
			return boughtTickets;
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

		private class RecommendedWeight
		{
			public double Weight { get; private set; }
			private readonly double _delta = 0.1;

			public RecommendedWeight()
			{
				Weight = 1.0;
			}

			public void IncreaseWeight()
			{
				Weight += _delta;
			}

			public void DecreaseWeight()
			{
				Weight -= _delta;
				if (Weight < 0.01) Weight = 0.01;
			}
		}
	}
}