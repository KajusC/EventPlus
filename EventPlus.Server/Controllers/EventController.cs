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
		public async Task<ActionResult<List<EventViewModel>>> GetTopRatedEvents([FromQuery] int? userId = null)
		{
			try
			{
				var events = await _eventLogic.GetAllEventsAsync();
				
				// Create a dictionary to map event IDs to their tasks for easier lookup
				var taskDict = new Dictionary<int, Task<double>>();
				foreach (var evt in events)
				{
					taskDict[evt.IdEvent] = CalculateEventWeight(evt, userId);
				}
				
				// Wait for all tasks to complete
				await Task.WhenAll(taskDict.Values);
				
				// Create a dictionary of event ID to weight for easy lookup
				var weightDict = new Dictionary<int, double>();
				foreach (var kvp in taskDict)
				{
					weightDict[kvp.Key] = kvp.Value.Result;
				}
				
				var recommendedEvents = new List<EventViewModel>();
				
				// Filter events with weight > 0.65
				foreach (var evt in events)
				{
					double weight = weightDict[evt.IdEvent];
					if (weight > 0.65)
					{
						recommendedEvents.Add(evt);
					}
				}
				
				// Sort by weight in descending order
				recommendedEvents = recommendedEvents
					.OrderByDescending(e => weightDict[e.IdEvent])
					.ToList();
				
				if (recommendedEvents.Count == 0)
				{
					// Return empty array with 200 status instead of 404
					return Ok(new List<EventViewModel>());
				}
				
				return Ok(recommendedEvents);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in GetTopRatedEvents: {ex.Message}");
				return StatusCode(500, $"Internal error: {ex.Message}");
			}
		}

		private async Task<double> CalculateEventWeight(EventViewModel eventEntity, int? userId = null)
		{
			if (eventEntity == null) return 0;

			try
			{
				var feedbacksTask = _feedbackLogic.GetFeedbacksByEventIdAsync(eventEntity.IdEvent);
				var organizerRatingTask = (eventEntity.FkOrganiseridUser > 0) ? 
					GetOrganizerRatingAsync(eventEntity.FkOrganiseridUser) : 
					Task.FromResult(0.7);
				var followerCountTask = (eventEntity.FkOrganiseridUser > 0) ? 
					GetOrganizerFollowerCountAsync(eventEntity.FkOrganiseridUser) : 
					Task.FromResult(50);
				
				Task<double> userCategoryPreferenceTask = Task.FromResult(0.0);
				if (userId.HasValue && eventEntity.Category.HasValue)
				{
					userCategoryPreferenceTask = GetUserCategoryPreferenceAsync(userId.Value, eventEntity.Category.Value);
				}
				
				Task<double> categoryWeightTask = Task.FromResult(0.0);
				if (eventEntity.Category.HasValue)
				{
					categoryWeightTask = GetCategoryWeightAsync(eventEntity.Category.Value, eventEntity.IdEvent);
				}
				
				await Task.WhenAll(feedbacksTask, organizerRatingTask, followerCountTask, categoryWeightTask, userCategoryPreferenceTask);
				
				var feedbacks = await feedbacksTask;
				double ratingWeight = 0.0;
				
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
				
				double categoryWeight = await categoryWeightTask;
				double userCategoryPreferenceWeight = await userCategoryPreferenceTask;
				
				double organizerWeight = 0.0;
				if (eventEntity.FkOrganiseridUser > 0)
				{
					var organizerRating = await organizerRatingTask;
					var followerCount = await followerCountTask;
					
					double followerScore = Math.Min(followerCount / 20000.0, 1.0);
					organizerWeight = (organizerRating * 0.7 + followerScore * 0.3) * 0.3;
					Console.WriteLine($"Organizer {eventEntity.FkOrganiseridUser} - Rating: {organizerRating:F2}, Followers: {followerCount}, Organizer Weight: {organizerWeight:F2}");
				}
				
				double totalWeight = 0.0;
				if (userId.HasValue && userCategoryPreferenceWeight > 0)
				{
					totalWeight = (ratingWeight * 0.4) +
								  (categoryWeight * 0.15) +
								  (organizerWeight * 0.25) +
								  (userCategoryPreferenceWeight * 0.2);
					Console.WriteLine($"Event '{eventEntity.Name}' - With user preference: {userCategoryPreferenceWeight:F2}, Total Weight: {totalWeight:F2}");
				}
				else
				{
					totalWeight = ratingWeight + categoryWeight + organizerWeight;
					Console.WriteLine($"Event '{eventEntity.Name}' - Total Weight: {totalWeight:F2}");
				}
				
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

		private async Task<double> GetCategoryWeightAsync(int categoryId, int currentEventId)
		{
			try
			{
				var eventsInCategory = await _eventLogic.GetEventsByCategoryAsync(categoryId);
				var categoryFeedbacks = new List<FeedbackViewModel>();
				
				foreach (var evt in eventsInCategory.Where(e => e.IdEvent != currentEventId))
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
						double categoryWeight = Convert.ToDouble(categoryAverage) / 10.0 * 0.2;
						Console.WriteLine($"Category {categoryId} - Average rating: {categoryAverage:F2}, Category Weight: {categoryWeight:F2}");
						return categoryWeight;
					}
				}
				
				return 0.0;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error calculating category weight: {ex.Message}");
				return 0.0;
			}
		}

		private async Task<double> GetUserCategoryPreferenceAsync(int userId, int categoryId)
		{
			try
			{
				// Get all feedbacks from this user
				var userFeedbacks = await _feedbackLogic.GetFeedbacksByUserIdAsync(userId);
				if (userFeedbacks == null || !userFeedbacks.Any())
				{
					return 0.0;
				}
				
				// Get all events to check categories
				var allEvents = await _eventLogic.GetAllEventsAsync();
				var eventsMap = allEvents.ToDictionary(e => e.IdEvent, e => e);
				
				// Filter feedbacks for events in the same category
				var categoryFeedbacks = userFeedbacks.Where(f => 
					eventsMap.ContainsKey(f.FkEventidEvent) && 
					eventsMap[f.FkEventidEvent].Category == categoryId && 
					f.Rating.HasValue
				).ToList();
				
				if (categoryFeedbacks.Any())
				{
					var userCategoryRatings = categoryFeedbacks.Select(f => f.Rating.Value);
					var averageUserCategoryRating = userCategoryRatings.Average();
					
					double userCategoryWeight = Convert.ToDouble(averageUserCategoryRating) / 10.0 * 0.2;
					
					Console.WriteLine($"User {userId} preference for Category {categoryId} - Average: {averageUserCategoryRating:F2}, Weight: {userCategoryWeight:F2}");
					return userCategoryWeight;
				}
				
				return 0.0;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error calculating user category preference: {ex.Message}");
				return 0.0;
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