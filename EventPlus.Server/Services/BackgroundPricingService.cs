using EventPlus.Server.Application.Handlers;
using EventPlus.Server.Application.IHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventPlus.Server.Services
{
	public class BackgroundPricingService : BackgroundService
	{
		private readonly ILogger<BackgroundPricingService> _logger;
		private readonly IServiceScopeFactory _scopeFactory; // Use scope factory instead of direct dependency
		private readonly TimeSpan _period = TimeSpan.FromHours(24); // Run every 24 hours

		public BackgroundPricingService(
			ILogger<BackgroundPricingService> logger,
			IServiceScopeFactory scopeFactory) // Change constructor parameter
		{
			_logger = logger;
			_scopeFactory = scopeFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Background Price Update Service is starting.");
			using PeriodicTimer timer = new(TimeSpan.FromHours(24));
			try
			{
				// Run the price update immediately when the service starts
				await RunPriceUpdateAsync();
				// Then run it every 24 hours
				while (await timer.WaitForNextTickAsync(stoppingToken))
				{
					await RunPriceUpdateAsync();
				}
			}
			catch (OperationCanceledException)
			{
				_logger.LogInformation("Background Price Update Service is stopping.");
			}
		}

		private async Task RunPriceUpdateAsync()
		{
			_logger.LogInformation("Running scheduled price update: {time}", DateTimeOffset.Now);
			try
			{
				using (var scope = _scopeFactory.CreateScope())
				{
					var _ticketLogic = scope.ServiceProvider.GetRequiredService<ITicketLogic>();

					var buyWeight = await _ticketLogic.InitiliazeBuyWeight(); //2
					var events = await _ticketLogic.CollectEventsData(); //3-4
					var eventList = events.Value;
					foreach (var e in eventList) // seq truksta loop?
					{
						var eventTickets = await _ticketLogic.FetchAllEventTickets(e.IdEvent);
						var eventSectorPrice = await _ticketLogic.FetchAllEventSectorPrices(e.IdEvent);
						var eventSameCategorySectorPrice = await _ticketLogic.CollectSameCategoryEventSectorPricesAsync(e.IdEvent);
						var organiser = await _ticketLogic.GetOrganiserByEvent(e.FkOrganiseridUser);
						var task1 = Task.Run(() => {
							var task1BW = buyWeight;
							var speedWeight = _ticketLogic.SoldEventTicketSpeed(e, eventTickets.Value);
							if (speedWeight > 10)
								task1BW = _ticketLogic.IncreaseBuyWeight(task1BW);
							else if (speedWeight < 1)
								task1BW = _ticketLogic.LowerBuyWeight(task1BW);

							var quantityWeight = _ticketLogic.RemainingEventTicketQuantity(e, eventTickets.Value);
							if (quantityWeight > 90)
								task1BW = _ticketLogic.LowerBuyWeight(task1BW);
							else if (quantityWeight < 20)
								task1BW = _ticketLogic.IncreaseBuyWeight(task1BW);

							var monthsUntilEvent = _ticketLogic.RemainingWaitingTime(e);
							if (monthsUntilEvent >= 6)
								task1BW = _ticketLogic.LowerBuyWeight(task1BW);
							else if (monthsUntilEvent < 1)
								task1BW = _ticketLogic.IncreaseBuyWeight(task1BW);
							return task1BW;
						});

						var task2 = Task.Run(() => {
							var mode = _ticketLogic.CalculateModeAndMultiply(eventSameCategorySectorPrice);
							return _ticketLogic.IncludeToWeight(buyWeight, mode);
						});

						var task3 = Task.Run(() => {
							double result = buyWeight;
							if (organiser.Rating < 3)
								result = _ticketLogic.LowerBuyWeight(result);
							else if (organiser.Rating > 8.5)
								result = _ticketLogic.IncreaseBuyWeight(result);
							if (organiser.FollowerCount < 1000)
								result = _ticketLogic.LowerBuyWeight(result);
							else if (organiser.FollowerCount > 100000)
								result = _ticketLogic.IncreaseBuyWeight(result);
							return result;
						});

						await Task.WhenAll(task1, task2, task3);
						var result1 = task1.Result - 1;
						var result2 = task2.Result - 1;
						var result3 = task3.Result - 1;
						var final = 1 + result1 + result2 + result3;
						await _ticketLogic.MultiplyWeightAndSectorPrices(e.IdEvent, final);
					}
				}
				_logger.LogInformation("Scheduled price update completed successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during scheduled price update");
			}
		}
	}
}