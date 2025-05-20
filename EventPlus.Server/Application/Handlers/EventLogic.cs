using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace EventPlus.Server.Application.Handlers
{
	public class EventLogic : IEventLogic
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ISectorLogic _sectorLogic;
		private readonly ISectorPriceLogic _sectorPriceLogic;
		private readonly ISeatingLogic _seatingLogic;

		public EventLogic(
			IUnitOfWork unitOfWork,
			IMapper mapper,
			ISectorLogic sectorLogic,
			ISectorPriceLogic sectorPriceLogic,
			ISeatingLogic seatingLogic)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_sectorLogic = sectorLogic;
			_sectorPriceLogic = sectorPriceLogic;
			_seatingLogic = seatingLogic;
		}

		public async Task<int> CreateEventAsync(EventViewModel eventEntity)
		{
			if (eventEntity == null)
			{
				throw new ArgumentNullException(nameof(eventEntity));
			}

			var eventEntityMapped = _mapper.Map<Event>(eventEntity);
			await _unitOfWork.Events.CreateAsync(eventEntityMapped);
			return eventEntityMapped.IdEvent;
		}

		public async Task<bool> DeleteEventAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
			}
			return await _unitOfWork.Events.DeleteAsync(id);
		}

		public async Task<List<EventViewModel>> GetAllEventsAsync()
		{
			var events = await _unitOfWork.Events.GetAllAsync();
			return _mapper.Map<List<EventViewModel>>(events);
		}

		public async Task<EventViewModel> GetEventByIdAsync(int id)
		{
			if (id <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
			}
			var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
			return _mapper.Map<EventViewModel>(eventEntity);
		}

		public async Task<bool> UpdateEventAsync(EventViewModel eventEntity)
		{
			if (eventEntity == null)
			{
				throw new ArgumentNullException(nameof(eventEntity));
			}

			if (!CheckEventFormData(eventEntity))
			{
				return false;
			}

			var eventEntityMapped = _mapper.Map<Event>(eventEntity);
			return await _unitOfWork.Events.UpdateAsync(eventEntityMapped);
		}

		private static bool CheckEventFormData(EventViewModel eventDTO)
		{
			if (string.IsNullOrWhiteSpace(eventDTO.Name))
			{
				return false;
			}
			if (eventDTO.StartDate == null || eventDTO.EndDate == null)
			{
				return false;
			}
			/*if (eventDTO.StartDate >= eventDTO.EndDate) KAS CIA ????????????????????? KODEL NEVEIKIA
			{
				Console.WriteLine(eventDTO.StartDate);
				Console.WriteLine(eventDTO.EndDate);
				return false;
			}*/

			if (eventDTO.MaxTicketCount < 0)
			{
				return false;
			}

			return true;
		}

		public async Task<int> CreateFullEvent(
			EventViewModel eventViewModel,
			EventLocationViewModel eventLocation,
			PartnerViewModel partner,
			PerformerViewModel performer,
			SectorViewModel sector,
			List<SectorViewModel> sectors = null,
			List<SectorPriceViewModel> sectorPrices = null,
			List<SeatingViewModel> seatings = null)
		{
			if (eventViewModel == null)
			{
				throw new ArgumentNullException(nameof(eventViewModel));
			}
			if (eventLocation == null && (sectors != null && sectors.Any()))
            {
                throw new ArgumentNullException(nameof(eventLocation), "EventLocation cannot be null if sectors are provided or if a new location is intended.");
            }

			try
			{
				if (!CheckEventFormData(eventViewModel))
				{
					throw new ArgumentException("Invalid event data.");
				}

				EventLocation locationToAssociate = null;
				int? finalLocationIdForEvent = null; // Laikys galutinį ID, kuris bus priskirtas renginiui

                if (eventLocation != null && eventLocation.IdEventLocation > 0)
                {
                    locationToAssociate = await _unitOfWork.EventLocations.GetByIdAsync(eventLocation.IdEventLocation);
                    if (locationToAssociate == null)
                    {
                        throw new KeyNotFoundException($"Supplied EventLocation ID {eventLocation.IdEventLocation} not found.");
                    }
                    finalLocationIdForEvent = locationToAssociate.IdEventLocation;
                }
                else if (eventLocation != null && !string.IsNullOrWhiteSpace(eventLocation.Name))
                {
                    // Kuriam naują vietą
                    var locationMapped = _mapper.Map<EventLocation>(eventLocation);
                    await _unitOfWork.EventLocations.CreateAsync(locationMapped);
                    await _unitOfWork.SaveAsync(); // <<< SVARBU: Išsaugome naują vietą, kad gautume jos ID
                    locationToAssociate = locationMapped;
                    finalLocationIdForEvent = locationToAssociate.IdEventLocation;
                }
                else if (eventLocation == null && (sectors == null || !sectors.Any()))
                {
                    // Leidžiama situacija, kai renginys neturi fizinės vietos IR neturi sektorių
                }
                else
                {
                    throw new ArgumentException("Event location details are missing or invalid for creating/assigning a location.");
                }

				
				if (finalLocationIdForEvent.HasValue)
                {
                    eventViewModel.FkEventLocationidEventLocation = finalLocationIdForEvent.Value;
                }
                else if (eventViewModel.FkEventLocationidEventLocation > 0) // Jei ID jau buvo nustatytas anksčiau
                {
                    var existingLocCheck = await _unitOfWork.EventLocations.GetByIdAsync(eventViewModel.FkEventLocationidEventLocation);
                    if (existingLocCheck == null)
                    {
                        throw new KeyNotFoundException($"EventViewModel.FkEventLocationidEventLocation {eventViewModel.FkEventLocationidEventLocation} refers to a non-existent location.");
                    }
                    // finalLocationIdForEvent lieka null, nes eventViewModel.FkEventLocationidEventLocation jau teisingas
                }


                // Dabar kuriam renginį. eventViewModel jau turi teisingą FkEventLocationidEventLocation.
                // CreateEventAsync turėtų taip pat išsaugoti ir grąžinti TIKRĄ ID.
                // Jei CreateEventAsync pats neišsaugo, reikėtų daryti panašiai:
                // var eventEntity = _mapper.Map<Event>(eventViewModel);
                // await _unitOfWork.Events.AddAsync(eventEntity);
                // await _unitOfWork.SaveAsync(); // <<< Išsaugome renginį
                // int eventId = eventEntity.IdEvent;
                // Vietoj to, jei CreateEventAsync jau tai daro:
                var eventId = await CreateEventAsync(eventViewModel);
                if (eventId <= 0)
                {
                    throw new Exception("Failed to create event or retrieve its ID.");
                }

                var createdSectors = new Dictionary<int, Sector>();

                if (sectors != null && sectors.Count > 0)
                {
                    // Patikriname, ar turime su kuo susieti sektorius
                    int? locationIdForSectors = locationToAssociate?.IdEventLocation ?? (eventViewModel.FkEventLocationidEventLocation > 0 ? eventViewModel.FkEventLocationidEventLocation : (int?)null);
                    if (!locationIdForSectors.HasValue)
                    {
                        throw new ArgumentException("Cannot create sectors without an associated event location ID.");
                    }

                    Console.WriteLine($"Processing {sectors.Count} sectors");

                    for (int i = 0; i < sectors.Count; i++)
                    {
                        var s = sectors[i];
                        s.FkEventLocationidEventLocation = locationIdForSectors.Value; // Naudojame jau gautą ID

                        Console.WriteLine($"Creating sector {i}: {s.Name}, Location ID: {s.FkEventLocationidEventLocation}");

                        var sectorCreated = await CreateSectors(s);
                        createdSectors.Add(i, sectorCreated);

                        Console.WriteLine($"Created sector with database ID: {sectorCreated.IdSector}");
                    }

					if (sectorPrices != null && sectorPrices.Count > 0)
					{
						Console.WriteLine($"Processing {sectorPrices.Count} sector prices");

						foreach (var price in sectorPrices)
						{
							Console.WriteLine($"Processing price {price.Price} for sector index {price.SectorId}");

							if (price.SectorId >= 0 && price.SectorId < sectors.Count)
							{
								var actualSectorId = createdSectors[(int)price.SectorId].IdSector;
								Console.WriteLine($"Linking price to sector ID: {actualSectorId}");

								var sectorPriceEntity = _mapper.Map<SectorPrice>(price);
								sectorPriceEntity.FkSectoridSector = actualSectorId;
								sectorPriceEntity.FkEventidEvent = eventId;

								await _unitOfWork.SectorPrices.CreateSectorPriceAsync(sectorPriceEntity);
							}
							else
							{
								Console.WriteLine($"Warning: Invalid sector index {price.SectorId} for price");
							}
						}
					}

					if (seatings != null && seatings.Count > 0)
					{
						Console.WriteLine($"Processing {seatings.Count} seatings");

						foreach (var seat in seatings)
						{
							Console.WriteLine($"Processing seat at row {seat.Row}, place {seat.Place} for sector index {seat.SectorId}");

							if (seat.SectorId >= 0 && seat.SectorId < sectors.Count)
							{
								var actualSectorId = createdSectors[(int)seat.SectorId].IdSector;
								Console.WriteLine($"Linking seat to sector ID: {actualSectorId}");

								var seatingEntity = _mapper.Map<Seating>(seat);
								seatingEntity.FkSectoridSector = actualSectorId;

								await _unitOfWork.Seatings.CreateSeatingAsync(seatingEntity);
							}
							else
							{
								Console.WriteLine($"Warning: Invalid sector index {seat.SectorId} for seating");
							}
						}
					}
				}
				else if (sector != null)
				{
					int? locationIdForSingleSector = locationToAssociate?.IdEventLocation ?? (eventViewModel.FkEventLocationidEventLocation > 0 ? eventViewModel.FkEventLocationidEventLocation : finalLocationIdForEvent);
                    if (!locationIdForSingleSector.HasValue)
                    {
                        throw new ArgumentException("Cannot create a single sector without an associated event location ID.");
                    }
                    sector.FkEventLocationidEventLocation = locationIdForSingleSector.Value;
                    await CreateSectors(sector);
                }

				if (partner != null && !string.IsNullOrEmpty(partner.Name))
				{
					var partnerMapped = _mapper.Map<Partner>(partner);
					await _unitOfWork.Partners.CreateAsync(partnerMapped);
				}

				if (performer != null && (!string.IsNullOrEmpty(performer.Name) || !string.IsNullOrEmpty(performer.Surname)))
				{
					var performerMapped = _mapper.Map<Performer>(performer);
					await _unitOfWork.Performers.CreateAsync(performerMapped);
				}

				await _unitOfWork.SaveAsync();
				return eventId;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in CreateFullEvent: {ex.Message}");
				Console.WriteLine($"Stack trace: {ex.StackTrace}");

				throw new ArgumentException($"Failed to create event: {ex.Message}", ex);
			}
		}

		public async Task<EventLocation> CreateEventLocation(EventLocationViewModel eventLocation)
		{
			if (eventLocation == null)
			{
				throw new ArgumentNullException(nameof(eventLocation));
			}

			var eventLocationMapped = _mapper.Map<EventLocation>(eventLocation);
			await _unitOfWork.EventLocations.CreateAsync(eventLocationMapped);
			await _unitOfWork.SaveAsync();
			return eventLocationMapped;
		}

		public async Task<Partner> CreatePartners(PartnerViewModel eventPartners)
		{
			if (eventPartners == null)
			{
				throw new ArgumentNullException(nameof(eventPartners));
			}
			var eventPartnersMapped = _mapper.Map<Partner>(eventPartners);
			await _unitOfWork.Partners.CreateAsync(eventPartnersMapped);
			return eventPartnersMapped;
		}

		public async Task<Performer> CreatePerformers(PerformerViewModel eventPerformers)
		{
			if (eventPerformers == null)
			{
				throw new ArgumentNullException(nameof(eventPerformers));
			}
			var eventPerformersMapped = _mapper.Map<Performer>(eventPerformers);
			await _unitOfWork.Performers.CreateAsync(eventPerformersMapped);
			return eventPerformersMapped;
		}

		public async Task<Sector> CreateSectors(SectorViewModel eventSectors)
		{
			if (eventSectors == null)
			{
				throw new ArgumentNullException(nameof(eventSectors));
			}

			var eventSectorsMapped = _mapper.Map<Sector>(eventSectors);
			await _unitOfWork.Sectors.CreateSectorAsync(eventSectorsMapped);
			await _unitOfWork.SaveAsync();
			return eventSectorsMapped;
		}

		public async Task<List<EventViewModel>> GetEventsByCategoryAsync(int categoryId)
		{
			if (categoryId <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(categoryId), "Category ID must be greater than zero.");
			}
			
			// Get all events
			var allEvents = await _unitOfWork.Events.GetAllAsync();
			
			// Filter by category
			var filteredEvents = allEvents.Where(e => e.Category == categoryId).ToList();
			
			return _mapper.Map<List<EventViewModel>>(filteredEvents);
		}

	public async Task<bool> InvalidateEventTicketsAsync(int eventId)
	{
		if (eventId <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
		}

		var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
		if (eventEntity == null)
		{
			throw new KeyNotFoundException($"Event with ID {eventId} not found.");
		}

		// Check if event has ended
		if (eventEntity.EndDate > DateOnly.FromDateTime(DateTime.Now))
		{
			return false;
		}

		var tickets = await _unitOfWork.Tickets.GetAllAsync();
		var eventTickets = tickets.Where(t => t.FkEventidEvent == eventId).ToList();

		foreach (var ticket in eventTickets)
		{
			await _unitOfWork.Tickets.UpdateTicketStatusInvalid(ticket.IdTicket);
		}

		await _unitOfWork.SaveAsync();
		return true;
	}

        public async Task<List<EventViewModel>> GetEventsByUserTicketsAsync(int userId)
        {
			var events = await _unitOfWork.Events.GetEventsByUserTicketsAsync(userId);
			return _mapper.Map<List<EventViewModel>>(events);
        }

		public async Task<List<EventViewModel>> GetEventsByOrganiserIdAsync(int organiserId)
		{
			var events = await _unitOfWork.Events.GetEventsByOrganiserIdAsync(organiserId);
			return _mapper.Map<List<EventViewModel>>(events);
		}

		public async Task<List<EventViewModel>> GetEventsByLocationIdAsync(int locationId) // Pataisytas metodas
        {
            var events = await _unitOfWork.Events.GetEventsByLocationIdAsync(locationId); // Tarkime, šis metodas grąžina Task<List<Event>>
            return _mapper.Map<List<EventViewModel>>(events);
        }

        public async Task<List<EventLocationViewModel>> GetSuggestedLocationsAsync(int? capacity, decimal? price, int? categoryId) // Implementuotas metodas
        {
            var allLocationEntities = await _unitOfWork.EventLocations.GetAllAsync();
            IEnumerable<EventLocation> query = allLocationEntities.AsEnumerable(); // Naudojame AsEnumerable LINQ to Objects operacijoms

            if (capacity.HasValue && capacity.Value > 0)
			{
				query = query.Where(loc => loc.Capacity >= capacity.Value);
			}
			else if (capacity.HasValue && capacity.Value == 0)
			{
				return new List<EventLocationViewModel>();
			}

            if (price.HasValue && price.Value > 0)
			{
				query = query.Where(loc => loc.Price <= (double)price.Value);
			}
			else if (price.HasValue && price.Value == 0)
			{
				return new List<EventLocationViewModel>();
			}

            if (categoryId.HasValue)
            {
                var allEvents = await _unitOfWork.Events.GetAllAsync();
                var relevantLocationIds = allEvents
                    .Where(ev => ev.Category == categoryId.Value && ev.FkEventLocationidEventLocation > 0)
                    .Select(ev => ev.FkEventLocationidEventLocation)
                    .Distinct()
                    .ToHashSet();

                query = query.Where(loc => relevantLocationIds.Contains(loc.IdEventLocation));
            }

            var resultLocations = query.ToList();
            return _mapper.Map<List<EventLocationViewModel>>(resultLocations);
        }
    }
}