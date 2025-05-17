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
			if (eventDTO.StartDate >= eventDTO.EndDate)
			{
				return false;
			}

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
			if (eventLocation == null)
			{
				throw new ArgumentNullException(nameof(eventLocation));
			}

			try
			{
				if (!CheckEventFormData(eventViewModel))
				{
					throw new ArgumentException("Invalid event data.");
				}

				var locationCreated = await CreateEventLocation(eventLocation);
				if (locationCreated == null)
				{
					throw new Exception("Failed to create event location.");
				}

				eventViewModel.FkEventLocationidEventLocation = locationCreated.IdEventLocation;

				var eventCreated = await CreateEventAsync(eventViewModel);
				if (eventCreated <= 0)
				{
					throw new Exception("Failed to create event.");
				}

				int eventId = eventCreated;

				var createdSectors = new Dictionary<int, Sector>();

				if (sectors != null && sectors.Count > 0)
				{
					Console.WriteLine($"Processing {sectors.Count} sectors");

					for (int i = 0; i < sectors.Count; i++)
					{
						var s = sectors[i];
						s.FkEventLocationidEventLocation = locationCreated.IdEventLocation;

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
					sector.FkEventLocationidEventLocation = locationCreated.IdEventLocation;
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
	}
}