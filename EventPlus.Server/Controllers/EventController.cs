using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
		public async Task<ActionResult<List<EventViewModel>>> GetTopRatedEvents()
		{
			try
			{
				var events = await _eventLogic.GetAllEventsAsync();
				
				var recommendedEvents = new List<EventViewModel>();
				
				foreach (var eventEntity in events)
				{
					double weight = await CalculateEventWeight(eventEntity);
					if (weight > 0.65)
					{
						recommendedEvents.Add(eventEntity);
					}
				}
				
				recommendedEvents = recommendedEvents.OrderByDescending(e => CalculateEventWeight(e).Result).ToList();
				
				if (recommendedEvents.Count == 0)
				{
					return NotFound("No highly rated events found. Try viewing all events instead.");
				}
				
				return Ok(recommendedEvents);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetTopRatedEvents: {ex.Message}");
				return StatusCode(500, $"Internal error: {ex.Message}");
			}
		}

		private async Task<double> CalculateEventWeight(EventViewModel eventEntity)
		{
			if (eventEntity == null) return 0;

			try
			{
				double ratingWeight = 0.5;
				double categoryWeight = 0.0;
				double organizerWeight = 0.0;
				
				var feedbacks = await _feedbackLogic.GetFeedbacksByEventIdAsync(eventEntity.IdEvent);
				Console.WriteLine($"Calculating weight for event '{eventEntity.Name}' with {feedbacks?.Count ?? 0} feedbacks");
				
				if (feedbacks != null && feedbacks.Any())
				{
					var ratings = feedbacks.Where(f => f.Rating.HasValue).Select(f => f.Rating.Value);
					if (ratings.Any())
					{
						var averageRating = ratings.Average();
						ratingWeight = Convert.ToDouble(averageRating) / 10.0 * 0.5;
						Console.WriteLine($"Event '{eventEntity.Name}' - Average rating: {averageRating:F2}, Rating Weight: {ratingWeight:F2}");
					}
				}
				
				if (eventEntity.Category.HasValue)
				{
					var eventsInCategory = await _eventLogic.GetEventsByCategoryAsync(eventEntity.Category.Value);
					var categoryFeedbacks = new List<FeedbackViewModel>();
					
					foreach (var evt in eventsInCategory.Where(e => e.IdEvent != eventEntity.IdEvent))
					{
						var evtFeedbacks = await _feedbackLogic.GetFeedbacksByEventIdAsync(evt.IdEvent);
						if (evtFeedbacks != null && evtFeedbacks.Any())
						{
							categoryFeedbacks.AddRange(evtFeedbacks);
						}
					}
					
					if (categoryFeedbacks.Any())
					{
						var categoryRatings = categoryFeedbacks.Where(f => f.Rating.HasValue).Select(f => f.Rating.Value);
						if (categoryRatings.Any())
						{
							var categoryAverage = categoryRatings.Average();
							categoryWeight = Convert.ToDouble(categoryAverage) / 10.0 * 0.2;
							Console.WriteLine($"Category {eventEntity.Category} - Average rating: {categoryAverage:F2}, Category Weight: {categoryWeight:F2}");
						}
					}
				}
				
				if (eventEntity.FkOrganiseridUser > 0)
				{
					var organizerRating = await GetOrganizerRatingAsync(eventEntity.FkOrganiseridUser);
					var followerCount = await GetOrganizerFollowerCountAsync(eventEntity.FkOrganiseridUser);
					
					double followerScore = Math.Min(followerCount / 20000.0, 1.0);

					organizerWeight = (organizerRating * 0.7 + followerScore * 0.3) * 0.3; 
					Console.WriteLine($"Organizer {eventEntity.FkOrganiseridUser} - Rating: {organizerRating:F2}, Followers: {followerCount}, Organizer Weight: {organizerWeight:F2}");
				}
				
				double totalWeight = ratingWeight + categoryWeight + organizerWeight;
				Console.WriteLine($"Event '{eventEntity.Name}' - Total Weight: {totalWeight:F2}");
				
				return totalWeight;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error calculating weight for event '{eventEntity.Name}': {ex.Message}");
				return 0.5;
			}
		}
		
		private async Task<double> GetOrganizerRatingAsync(int organizerId)
		{
			try
			{
				var organizer = await _organiserLogic.GetOrganiserByIdAsync(organizerId);
				
				if (organizer != null && organizer.Rating.HasValue)
				{
					return Math.Min(Math.Max(organizer.Rating.Value / 10.0, 0), 1);
				}
				
				Console.WriteLine($"No rating found for organizer {organizerId}, using default value");
				return 0.7;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching organizer rating: {ex.Message}");
				return 0.7;
			}
		}
		
		private async Task<int> GetOrganizerFollowerCountAsync(int organizerId)
		{
			try
			{
				var organizer = await _organiserLogic.GetOrganiserByIdAsync(organizerId);
				
				if (organizer != null && organizer.FollowerCount.HasValue)
				{
					return organizer.FollowerCount.Value;
				}
				
				Console.WriteLine($"No follower count found for organizer {organizerId}, using default value");
				return 50;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching organizer follower count: {ex.Message}");
				return 50;
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