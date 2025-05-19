using eventplus.models.Domain.Events;
using eventplus.models.Domain.Tickets;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        Task<byte[]> GenerateTicketPdfAsync(int ticketId);
        Task<TicketValidationResult> DecryptQrCode(string qrCode);
        Task<bool> UpdateTicketScanTime(int ticketId);
        Task<List<TicketViewModel>> GetTicketsByUserIdAsync(int userId);
        Task<byte[]> GenerateTicketPdfAsync(int ticketId);
        Task<TicketValidationResult> DecryptQrCode(string qrCode);
        Task<bool> UpdateTicketScanTime(int ticketId);
        Task<List<TicketViewModel>> GetTicketsByUserIdAsync(int userId);
        Task<double> InitiliazeBuyWeight();
        Task<ActionResult<List<EventViewModel>>> CollectEventsData();
        Task<ActionResult<List<TicketViewModel>>> FetchAllEventTickets(int eventId);
        Task<ActionResult<List<SectorPriceViewModel>>> FetchAllEventSectorPrices(int eventId);
        Task MultiplyWeightAndSectorPrices(int eventId, double buyWeight);
        double RemainingEventTicketQuantity(EventViewModel eventData, List<TicketViewModel> tickets);
		double SoldEventTicketSpeed(EventViewModel eventData, List<TicketViewModel> tickets, int monthPeriod = 1);
        double RemainingWaitingTime(EventViewModel eventData);
        Task<List<double>> CollectSameCategoryEventSectorPricesAsync(int? categoryId);

		double IncludeToWeight(double buyWeight, double adjustment);
        double IncreaseBuyWeight(double buyWeight, double weight = 0.1);
        double LowerBuyWeight(double buyWeight, double weight = 0.1);
        double CalculateModeAndMultiply(List<double> prices);
        Task<OrganiserViewModel> GetOrganiserByEvent(int organiserId);
	}
}
