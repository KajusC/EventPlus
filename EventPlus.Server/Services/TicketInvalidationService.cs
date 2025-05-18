using EventPlus.Server.Application.IHandlers;
using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace EventPlus.Server.Services
{
    public class TicketInvalidationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TicketInvalidationService> _logger;
        private readonly PeriodicTimer _timer;

        public TicketInvalidationService(
            IServiceProvider serviceProvider,
            ILogger<TicketInvalidationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _timer = new PeriodicTimer(TimeSpan.FromHours(12));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Ticket Invalidation Service is starting.");

            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
                {
                    await InvalidateTicketsForEndedEvents();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Ticket Invalidation Service is stopping.");
            }
        }

        private async Task InvalidateTicketsForEndedEvents()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var ticketLogic = scope.ServiceProvider.GetRequiredService<ITicketLogic>();

                // Get all events that have ended
                var endedEvents = await unitOfWork.Events.GetAllAsync();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var endedEventsList = endedEvents.Where(e => e.EndDate.HasValue && e.EndDate.Value < today).ToList();

                foreach (var endedEvent in endedEventsList)
                {
                    _logger.LogInformation($"Processing ended event: {endedEvent.Name} (ID: {endedEvent.IdEvent})");

                    // Get all tickets for this event
                    var tickets = await unitOfWork.Tickets.GetAllAsync();
                    var eventTickets = tickets.Where(t => t.FkEventidEvent == endedEvent.IdEvent).ToList();

                    foreach (var ticket in eventTickets)
                    {
                        try
                        {
                            await unitOfWork.Tickets.UpdateTicketStatusInvalid(ticket.IdTicket);
                            _logger.LogInformation($"Invalidated ticket {ticket.IdTicket} for event {endedEvent.IdEvent}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error invalidating ticket {ticket.IdTicket} for event {endedEvent.IdEvent}");
                        }
                    }
                }

                await unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while invalidating tickets for ended events");
            }
        }
    }
}