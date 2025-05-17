using AutoMapper;
using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.Persistance.IRepositories;
using eventplus.models.Infrastructure.UnitOfWork;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.Handlers
{
    public class TicketLogic : ITicketLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketLogic(
            IUnitOfWork unitOfWork,
            ITicketRepository ticketRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
    }
}