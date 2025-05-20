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

		private const double EVENT_COUNT_AVG_THRESHOLD_INCREASE = 3.0;
        private const double EVENT_COUNT_AVG_THRESHOLD_DECREASE = 1.0;
        private const int PAST_EVENTS_REVIEW_COUNT_THRESHOLD_INCREASE = 30;
        private const int PAST_EVENTS_REVIEW_COUNT_THRESHOLD_DECREASE = 10;
        private const double VIEWERS_AVG_THRESHOLD_INCREASE = 200.0;
        private const double VIEWERS_AVG_THRESHOLD_DECREASE = 50.0;
        private const double SUITABILITY_INDEX_INCREMENT = 0.2;
        private const double SUITABILITY_INDEX_DECREMENT = 0.1;
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
					return Task.Run(async () =>
					{
						var feedbackTask = Task.Run(() =>
						{
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
						});

						var organiserTask = Task.Run(() =>
						{
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
							var userBoughtTickets = tickets.Where(t => t.FkUseridUser == userId).ToList();
							var userEventCategories = userBoughtTickets
								.Select(t => hashMap[t.FkEventidEvent]?.Category)
								.Where(c => c != null)
								.Distinct()
								.ToList();

							var similarCategoryEvents = allEvents.Where(e => userEventCategories.Contains(e.Category)).ToList();
							var similarCategoryEventFeedbacks = allFeedbacks.Where(f => similarCategoryEvents.Any(e => e.IdEvent == f.FkEventidEvent)).ToList();
							if (similarCategoryEventFeedbacks.Any())
							{
								var avgRating = similarCategoryEventFeedbacks.Average(f => f.Rating ?? 0);
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
						});

						await Task.WhenAll(feedbackTask, organiserTask);

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

		 [HttpGet("location/{id}/ratings")]
        public async Task<ActionResult<List<FeedbackViewModel>>> GetLocationRatings(int id)
        {
            var ratings = await _feedbackLogic.GetFeedbackByLocationIdAsync(id);
            return Ok(ratings);
        }


        [HttpPost("venue-suitability")]
        [Authorize(Roles = "Organiser,Administrator")]
        public async Task<ActionResult<List<EventLocationViewModel>>> GetVenueSuitabilityRecommendations([FromBody] EventViewModel eventFormData)
        {
            if (!CheckEventFormDataInternal(eventFormData))
            {
                return BadRequest("Invalid event form data for suitability check.");
            }

            var locationSuitabilityScores = new Dictionary<int, double>();

            var initialSuitabilityLocations = await GetSuitableLocationListInternal(eventFormData);

            if (initialSuitabilityLocations == null || !initialSuitabilityLocations.Any())
            {
                return Ok(new { Message = "No suitable venues found based on initial criteria.", RecommendedVenues = new List<EventLocationViewModel>() });
            }

            foreach (var loc in initialSuitabilityLocations)
            {
                locationSuitabilityScores[loc.IdEventLocation] = 1.0; // Base score
            }
            
            foreach (var location in initialSuitabilityLocations)
            {
                var pastEvents = await GetPastEventsDataInternal(location.IdEventLocation);

                if (pastEvents.Any())
                {
                    double avgEventCount = EvaluateEventCountAvgInternal(pastEvents);
                    if (avgEventCount > EVENT_COUNT_AVG_THRESHOLD_INCREASE) // e.g., if avg events > 3
                    {
                        IncreaseSuitabilityIndexInternal(locationSuitabilityScores, location.IdEventLocation, SUITABILITY_INDEX_INCREMENT);
                    }
                    else if (avgEventCount < EVENT_COUNT_AVG_THRESHOLD_DECREASE) // e.g., if avg events < 1
                    {
                        DecreaseSuitabilityIndexInternal(locationSuitabilityScores, location.IdEventLocation, SUITABILITY_INDEX_DECREMENT);
                    }

                    var eventIdsAtLocation = pastEvents.Select(e => e.IdEvent).ToList();
                    var feedbacksForLocationEvents = await GetDataInternal_Feedback(eventIdsAtLocation);

                    if (feedbacksForLocationEvents.Any())
                    {
                        int reviewCount = EvaluatePastEventsReviewCountInternal(feedbacksForLocationEvents);
                        if (reviewCount > PAST_EVENTS_REVIEW_COUNT_THRESHOLD_INCREASE) // e.g., if review count > 30
                        {
                            IncreaseSuitabilityIndexInternal(locationSuitabilityScores, location.IdEventLocation, SUITABILITY_INDEX_INCREMENT);
                        }
                        else if (reviewCount < PAST_EVENTS_REVIEW_COUNT_THRESHOLD_DECREASE) // e.g., if review count < 10
                        {
                            DecreaseSuitabilityIndexInternal(locationSuitabilityScores, location.IdEventLocation, SUITABILITY_INDEX_DECREMENT);
                        }
                    }

                    var ticketsForLocationEvents = await GetSoldTicketsInternal(eventIdsAtLocation); // Assuming this gets all relevant tickets

                    if (ticketsForLocationEvents.Any())
                    {
                        double avgViewers = EvaluateViewersAvgInternal(ticketsForLocationEvents);
                        if (avgViewers > VIEWERS_AVG_THRESHOLD_INCREASE) // e.g., if avg viewers > 200
                        {
                            IncreaseSuitabilityIndexInternal(locationSuitabilityScores, location.IdEventLocation, SUITABILITY_INDEX_INCREMENT);
                        }
                        else if (avgViewers < VIEWERS_AVG_THRESHOLD_DECREASE) // e.g., if avg viewers < 50
                        {
                            DecreaseSuitabilityIndexInternal(locationSuitabilityScores, location.IdEventLocation, SUITABILITY_INDEX_DECREMENT);
                        }
                    }
                }
            }

            var recommendedVenues = CreatePlaceListInternal(initialSuitabilityLocations, locationSuitabilityScores);

            if (!recommendedVenues.Any())
            {
                 return Ok(new { Message = "Not Found: No venues meet the refined suitability criteria.", RecommendedVenues = new List<EventLocationViewModel>() });
            }
            
            return Ok(new { Message = "Venue recommendations generated.", RecommendedVenues = recommendedVenues });
        }

        private bool CheckEventFormDataInternal(EventViewModel eventData)
        {
            if (eventData == null) return false;
            if (string.IsNullOrWhiteSpace(eventData.Name)) return false;
            if (eventData.StartDate == null || eventData.EndDate == null) return false;
            if (eventData.MaxTicketCount < 0) return false;
            if (eventData.Category == null || eventData.Category <= 0) return false;
            return true;
        }
        private async Task<List<EventLocationViewModel>> GetSuitableLocationListInternal(EventViewModel eventData)
        {
            return await _eventLogic.GetSuggestedLocationsAsync(
                eventData.MaxTicketCount,
                eventData.Budget,
                eventData.Category
            );
        }

        private async Task<List<EventViewModel>> GetPastEventsDataInternal(int locationId)
        {
            return await _eventLogic.GetEventsByLocationIdAsync(locationId);
        }

        private double EvaluateEventCountAvgInternal(List<EventViewModel> pastEvents)
        {
            if (pastEvents == null || !pastEvents.Any()) return 0.0;
            return pastEvents.Count; 
        }

        private void IncreaseSuitabilityIndexInternal(Dictionary<int, double> scores, int locationId, double amount)
        {
            if (scores.ContainsKey(locationId))
            {
                scores[locationId] += amount;
            }
        }

        private void DecreaseSuitabilityIndexInternal(Dictionary<int, double> scores, int locationId, double amount)
        {
            if (scores.ContainsKey(locationId))
            {
                scores[locationId] -= amount;
                if (scores[locationId] < 0) scores[locationId] = 0;
            }
        }
        private async Task<List<FeedbackViewModel>> GetDataInternal_Feedback(List<int> eventIds)
        {
            var allFeedbacks = new List<FeedbackViewModel>();
            if (eventIds == null || !eventIds.Any()) return allFeedbacks;

            foreach (var eventId in eventIds)
            {
                var feedbacks = await _feedbackLogic.GetFeedbacksByEventIdAsync(eventId);
                if (feedbacks != null) allFeedbacks.AddRange(feedbacks);
            }
            return allFeedbacks;
        }
        
        private int EvaluatePastEventsReviewCountInternal(List<FeedbackViewModel> feedbacks)
        {
            return feedbacks?.Count ?? 0;
        }

        private async Task<List<TicketViewModel>> GetSoldTicketsInternal(List<int> eventIdsAtLocation)
        {
            var allTickets = new List<TicketViewModel>();
            if (eventIdsAtLocation == null || !eventIdsAtLocation.Any()) return allTickets;

            var allSystemTickets = await _ticketLogic.GetAllTicketsAsync();
            foreach (var eventId in eventIdsAtLocation)
            {
                allTickets.AddRange(allSystemTickets.Where(t => t.FkEventidEvent == eventId));
            }
            return allTickets;
        }
        
        private double EvaluateViewersAvgInternal(List<TicketViewModel> tickets)
        {
            if (tickets == null || !tickets.Any()) return 0.0;
            var ticketsByEvent = tickets.GroupBy(t => t.FkEventidEvent);
            if (!ticketsByEvent.Any()) return 0.0;
            return ticketsByEvent.Average(g => g.Count());
        }

        private List<EventLocationViewModel> CreatePlaceListInternal(List<EventLocationViewModel> allLocations, Dictionary<int, double> scores)
        {
            return allLocations
                .Where(loc => scores.ContainsKey(loc.IdEventLocation) && scores[loc.IdEventLocation] > 0) 
                .OrderByDescending(loc => scores[loc.IdEventLocation])
                .ToList();
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