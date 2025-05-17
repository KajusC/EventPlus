using EventPlus.Server.Application.ViewModels;

namespace EventPlus.Server.Application.IHandlers
{
    public interface ITicketLogic
    {
        Task<bool> CreateTicketAsync(TicketViewModel ticket);
        Task<bool> UpdateTicketAsync(TicketViewModel ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<TicketViewModel> GetTicketByIdAsync(int id);
        Task<List<TicketViewModel>> GetAllTicketsAsync();
        Task<bool> IfEventHasTickets(int eventId);
        Task<TicketValidationResult> DecryptQrCode(string qrCode);
        Task<bool> UpdateTicketScanTime(int ticketId);
    }
}
