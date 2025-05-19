using AutoMapper;
using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace EventPlus.Server.Application.Handlers
{
    public class TicketLogic : ITicketLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITicketRepository _ticketRepository;
        private readonly IOrganiserRepository _organiserRepository;
        private readonly ISectorPriceRepository _sectorPriceRepository;

        public TicketLogic(
            IUnitOfWork unitOfWork, IMapper mapper,
			ITicketRepository ticketRepository,
            IOrganiserRepository organiserRepository,
			ISectorPriceRepository sectorPriceRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
			_ticketRepository = ticketRepository;
            _organiserRepository = organiserRepository;
            _sectorPriceRepository = sectorPriceRepository;
        }

        public async Task<bool> CreateTicketAsync(TicketViewModel ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var ticketEntity = _mapper.Map<Ticket>(ticket);
            return await _unitOfWork.Tickets.CreateAsync(ticketEntity);
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            return await _unitOfWork.Tickets.DeleteAsync(id);
        }

        public async Task<List<TicketViewModel>> GetAllTicketsAsync()
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return _mapper.Map<List<TicketViewModel>>(tickets);
        }

        public async Task<TicketViewModel> GetTicketByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }
            var ticketEntity = await _unitOfWork.Tickets.GetByIdAsync(id);
            return _mapper.Map<TicketViewModel>(ticketEntity);
        }

        public async Task<bool> UpdateTicketAsync(TicketViewModel ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }
            var ticketEntity = _mapper.Map<Ticket>(ticket);
            return await _unitOfWork.Tickets.UpdateAsync(ticketEntity);
        }

        public async Task<bool> IfEventHasTickets(int eventId)
        {
            if (eventId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(eventId), "Event ID must be greater than zero.");
            }
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return tickets.Any(t => t.FkEventidEvent == eventId);
        }
        
        public async Task<byte[]> GenerateTicketPdfAsync(int ticketId)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ticketId), "Ticket ID must be greater than zero.");
            }
            
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with ID {ticketId} not found.");
            }
            
            var eventInfo = await _unitOfWork.Events.GetByIdAsync(ticket.FkEventidEvent);
            if (eventInfo == null)
            {
                throw new KeyNotFoundException($"Event with ID {ticket.FkEventidEvent} not found.");
            }
            var seating = await _unitOfWork.Seatings.GetSeatingByIdAsync(ticket.FkSeatingidSeating);
            if (seating == null)
            {
                throw new KeyNotFoundException($"Seating with ID {ticket.FkSeatingidSeating} not found.");
            }
            
            var pdfStream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 50, 50);
            var writer = PdfWriter.GetInstance(document, pdfStream);
            document.Open();

            // Pridėti antraštę
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
            var title = new Paragraph($"Event Ticket: {eventInfo?.Name ?? "Event #" + ticket.FkEventidEvent}", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            document.Add(title);
            document.Add(new Paragraph(" ")); // Add space

            // Pridėti bilieto informaciją
            var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            document.Add(new Paragraph($"Ticket ID: {ticket.IdTicket}", contentFont));
            document.Add(new Paragraph($"Event: {eventInfo?.Name ?? "Unknown Event"}", contentFont));
            document.Add(new Paragraph($"Date: {ticket.GenerationDate}", contentFont));
            document.Add(new Paragraph($"Price: {ticket.Price} €", contentFont));
            document.Add(new Paragraph($"Seat: Row {seating?.Row ?? 0}, Place {seating?.Place ?? 0}", contentFont));
            document.Add(new Paragraph($"Status: {ticket.FkTicketstatusNavigation?.Name ?? ticket.FkTicketstatus.ToString()}", contentFont));
            document.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", contentFont));
            document.Add(new Paragraph(" ")); // Add space

            // Generuoti QR kodą
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(ticket.QrCode, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            // Pridėti QR kodą į dokumentą
            using (var qrCodeStream = new MemoryStream(qrCodeBytes))
            {
                var qrImage = Image.GetInstance(qrCodeStream);
                qrImage.Alignment = Element.ALIGN_CENTER;
                qrImage.ScaleToFit(150, 150);
                document.Add(qrImage);
            }

            // Pridėti validacijos tekstą
            var validationFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 10);
            var validationText = new Paragraph("This ticket is valid only when presented with ID. QR code will be scanned at entrance.", validationFont);
            validationText.Alignment = Element.ALIGN_CENTER;
            document.Add(validationText);

            document.Close();
            writer.Close();
            return pdfStream.ToArray();
        }
        

        public async Task<TicketValidationResult> DecryptQrCode(string qrCode)
        {
            if (string.IsNullOrEmpty(qrCode))
            {
                throw new ArgumentNullException(nameof(qrCode), "QR code cannot be null or empty.");
            }

            try
            {
                var ticketId = await _unitOfWork.Tickets.GetIdByQrCode(qrCode);
                var ticket_data = await _unitOfWork.Tickets.GetByIdAsync(ticketId);

                await ValidateTicketData(ticket_data);

                var lastScannedDate = ticket_data.ScannedDate ?? DateTime.MinValue;

                var ticketGotScanned = ticket_data.ScannedDate != null &&
                    ticket_data.ScannedDate != DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Unspecified);

                var ticketStatus = ticket_data.FkTicketstatusNavigation?.Name switch
                {
                    "Scanned" => TicketStatus.Scanned,
                    "Inactive" => TicketStatus.Inactive,
                    _ => TicketStatus.Valid
                };


                if (ticketGotScanned || ticketStatus == TicketStatus.Scanned)
                {
                    return new TicketValidationResult
                    {
                        IsValid = false,
                        Message = "Ticket already scanned",
                        Ticket = new TicketViewModel
                        {
                            IdTicket = ticket_data.IdTicket,
                            FkEventidEvent = ticket_data.FkEventidEvent,
                            SeatingId = ticket_data.FkSeatingidSeating,
                            TicketStatusId = ticket_data.FkTicketstatus,
                            Price = ticket_data.Price,
                            GenerationDate = ticket_data.GenerationDate,
                            ScannedDate = lastScannedDate,
                        }
                    };
                }

                if (ticketStatus == TicketStatus.Inactive)
                {
                    return new TicketValidationResult
                    {
                        IsValid = false,
                        Message = "Ticket is INVALID",
                        Ticket = new TicketViewModel
                        {
                            IdTicket = ticket_data.IdTicket,
                            FkEventidEvent = ticket_data.FkEventidEvent,
                            SeatingId = ticket_data.FkSeatingidSeating,
                            TicketStatusId = ticket_data.FkTicketstatus,
                            Price = ticket_data.Price,
                            GenerationDate = ticket_data.GenerationDate,
                            ScannedDate = lastScannedDate,
                        }
                    };
                }

                await _unitOfWork.Tickets.UpdateTicketStatusRead(ticket_data.IdTicket);

                if (ticketStatus == TicketStatus.Valid)
                {
                    return new TicketValidationResult
                    {
                        IsValid = true,
                        Message = "Ticket is VALID",
                        Ticket = new TicketViewModel
                        {
                            IdTicket = ticket_data.IdTicket,
                            FkEventidEvent = ticket_data.FkEventidEvent,
                            SeatingId = ticket_data.FkSeatingidSeating,
                            TicketStatusId = ticket_data.FkTicketstatus,
                            Price = ticket_data.Price,
                            GenerationDate = ticket_data.GenerationDate,
                            ScannedDate = lastScannedDate,
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TicketValidationResult
                {
                    IsValid = false,
                    Message = "Ticket not found",
                    Ticket = new TicketViewModel()
                };
            }

            return new TicketValidationResult
            {
                IsValid = false,
                Message = "Ticket is INVALID AT ALL",
                Ticket = new TicketViewModel()
            };
        }

        public Task<bool> UpdateTicketScanTime(int ticketId)
        {
            if (ticketId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ticketId), "Ticket ID must be greater than zero.");
            }
            return _unitOfWork.Tickets.UpdateTicketScanDate(ticketId);
        }

        public async Task<List<TicketViewModel>> GetTicketsByUserIdAsync(int userId)
        {
            var tickets = await _unitOfWork.Tickets.GetTicketsByUserIdAsync(userId);
            return _mapper.Map<List<TicketViewModel>>(tickets);
        }

        private async Task<bool> ValidateTicketData(Ticket ticket)
        {
            if (ticket == null)
            {
                return false;
            }
            if (ticket.FkEventidEvent <= 0)
            {
                await _unitOfWork.Tickets.UpdateTicketStatusInvalid(ticket.IdTicket);
                return false;
            }
            if (ticket.FkSeatingidSeating <= 0)
            {
                await _unitOfWork.Tickets.UpdateTicketStatusInvalid(ticket.IdTicket);
                return false;
            }
            if (ticket.Price <= 0)
            {
                await _unitOfWork.Tickets.UpdateTicketStatusInvalid(ticket.IdTicket);
                return false;
            }
            return true;
        }

        private enum TicketStatus
        {
            Scanned = 0,
            Inactive = 1,
            Valid = 2
        }
		public async Task<double> InitiliazeBuyWeight()
		{
			return 1.0;//2
		}
		public async Task<ActionResult<List<EventViewModel>>> CollectEventsData()
		{
			var events = await _unitOfWork.Events.GetAllAsync(); //3
			return _mapper.Map<List<EventViewModel>>(events); //4
		}
        public async Task<ActionResult<List<TicketViewModel>>> FetchAllEventTickets(int eventId)
        {
            var tickets = await _ticketRepository.GetTicketsByEventIdAsync(eventId);
            return _mapper.Map<List<TicketViewModel>>(tickets);
        }
		public async Task<ActionResult<List<SectorPriceViewModel>>> FetchAllEventSectorPrices(int eventId)
		{
			var sectorPrices = await _ticketRepository.GetSectorPricesByEventIdAsync(eventId);
			return _mapper.Map<List<SectorPriceViewModel>>(sectorPrices);
		}
		public double RemainingWaitingTime(EventViewModel eventData)
        {
			var currentDate = DateOnly.FromDateTime(DateTime.Today);
			var startDate = eventData.StartDate ?? currentDate;

			var totalDays = (startDate.ToDateTime(TimeOnly.MinValue) - currentDate.ToDateTime(TimeOnly.MinValue)).TotalDays;
			var monthsUntilEvent = totalDays / 30;
            return monthsUntilEvent;
		}

		public double RemainingEventTicketQuantity(EventViewModel eventData, List<TicketViewModel> tickets)
        {
			var totalTicketCount = eventData.MaxTicketCount ?? 0;
			var soldTicketCount = tickets.Count;
			var unsoldTicketCount = totalTicketCount - soldTicketCount;
			var unsoldPercentage = totalTicketCount > 0 ? (unsoldTicketCount / (double)totalTicketCount) * 100 : 0;
			return unsoldPercentage;
        }
        public double SoldEventTicketSpeed(EventViewModel eventData, List<TicketViewModel> tickets, int monthPeriod = 1)
        {
            var totalTicketCount = eventData.MaxTicketCount;
			var oneMonthAgo = DateTime.Today.AddMonths(-monthPeriod);
			var recentlySoldTickets = tickets.Where(t =>
				t.GenerationDate.HasValue &&
				DateOnly.FromDateTime(oneMonthAgo) <= t.GenerationDate.Value).ToList();

			var soldPercentage = totalTicketCount > 0 ? (recentlySoldTickets.Count / (double)totalTicketCount) * 100 : 0;
            return soldPercentage;
		}
		public double IncreaseBuyWeight(double buyWeight, double weight = 0.1)
		{
            return buyWeight += weight;
		}
		public double LowerBuyWeight(double buyWeight, double weight = 0.1)
		{
			return buyWeight -= weight;
		}
		public async Task<List<double>> CollectSameCategoryEventSectorPricesAsync(int? categoryId)
		{
			int catId = categoryId ?? 0;

			var events = await _ticketRepository.GetSameCategoryEventSectorPricesAsync(catId);
			var eventData = _mapper.Map<List<EventViewModel>>(events);
			var categoryPrices = new List<double>();

			foreach (var categoryEvent in eventData)
			{
				var sectorPrices = await _ticketRepository.GetSectorPricesByEventIdAsync(categoryEvent.IdEvent);

				var prices = sectorPrices
					.Select(sp => sp.Price ?? 0)
					.Where(p => p > 0)
					.ToList();

				categoryPrices.AddRange(prices);
			}

			return categoryPrices;
		}


		public double CalculateModeAndMultiply(List<double> prices)
		{
            if (prices == null || !prices.Any())
                return 0;

			var priceFrequency = new Dictionary<int, int>();
			var maxFrequency = 0;
			var mode = 0;

			foreach (var price in prices)
			{
				var roundedPrice = (int)Math.Round(price);
				if (!priceFrequency.ContainsKey(roundedPrice))
				{
					priceFrequency[roundedPrice] = 0;
				}

				priceFrequency[roundedPrice]++;

				if (priceFrequency[roundedPrice] > maxFrequency)
				{
					maxFrequency = priceFrequency[roundedPrice];
					mode = roundedPrice;
				}
			}
			var modeAdjustment = mode * 0.001;
			return (modeAdjustment);
		}

		public double IncludeToWeight(double buyWeight, double adjustment)
		{
			return buyWeight += adjustment;
		}
		public async Task<OrganiserViewModel> GetOrganiserByEvent(int organiserId)
        {
			var organiser = await _organiserRepository.GetByIdAsync(organiserId);
			return _mapper.Map<OrganiserViewModel>(organiser);
		}

		public async Task MultiplyWeightAndSectorPrices(int eventId, double buyWeight)
		{
            var sectorPrices = await _ticketRepository.GetSectorPricesByEventIdAsync(eventId);
			if (sectorPrices == null || !sectorPrices.Any())
				return;

			foreach (var sectorPrice in sectorPrices)
			{
				if (sectorPrice.Price.HasValue)
				{
					sectorPrice.Price = Math.Round(sectorPrice.Price.Value * buyWeight, 2);
					await _sectorPriceRepository.UpdateSectorPriceAsync(sectorPrice);
				}
			}
		}

	}
}