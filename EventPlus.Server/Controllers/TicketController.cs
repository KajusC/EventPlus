using eventplus.models.Domain.Events;
using eventplus.models.Domain.Sectors;
using eventplus.models.Domain.Tickets;
using EventPlus.Server.Application.Handlers;
using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketLogic _ticketLogic;
		private readonly IEventLogic _eventLogic;
		private readonly ISectorPriceLogic _sectorPriceLogic;
		private readonly IOrganiserLogic _organiserLogic;


		public TicketController(
			ITicketLogic ticketLogic,
			IEventLogic eventLogic,
			ISectorPriceLogic sectorPriceLogic,
			IOrganiserLogic organiserLogic)
		{
			_ticketLogic = ticketLogic;
			_eventLogic = eventLogic;
			_sectorPriceLogic = sectorPriceLogic;
			_organiserLogic = organiserLogic;
		}
		[HttpGet]
        public async Task<ActionResult<List<TicketViewModel>>> GetAllTickets()
        {
            var tickets = await _ticketLogic.GetAllTicketsAsync();
            return Ok(tickets);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicketById(int id)
        {
            var ticketEntity = await _ticketLogic.GetTicketByIdAsync(id);
            if (ticketEntity == null)
            {
                return NotFound();
            }
            return Ok(ticketEntity);
        }
        [HttpPost]
        public async Task<ActionResult<bool>> CreateTicket([FromBody] TicketViewModel ticketDTO)
        {
            if (ticketDTO == null)
            {
                return BadRequest("Ticket cannot be null");
            }
            var result = await _ticketLogic.CreateTicketAsync(ticketDTO);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticketDTO.IdTicket }, result);
        }
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateTicket([FromBody] TicketViewModel ticketDTO)
        {
            if (ticketDTO == null)
            {
                return BadRequest("Ticket cannot be null");
            }
            var result = await _ticketLogic.UpdateTicketAsync(ticketDTO);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTicket(int id)
        {
            var result = await _ticketLogic.DeleteTicketAsync(id);
            return Ok(result);
        }

        [HttpGet("generatePdf/{ticketId}")]
        public async Task<IActionResult> GenerateTicketPdf(int ticketId)
        {
            try
            {
                var pdfBytes = await _ticketLogic.GenerateTicketPdfAsync(ticketId);
                return File(pdfBytes, "application/pdf", $"ticket_{ticketId}.pdf");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Ticket not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating ticket: {ex.Message}");
            }
        }       
        [HttpPost("ScanQrCode")]
        public async Task<ActionResult<TicketValidationResult>> ScanQrCode([FromBody] string qrCode)
        {
            var result = await _ticketLogic.DecryptQrCode(qrCode);

            if (result.Ticket?.IdTicket > 0)
            {
                await _ticketLogic.UpdateTicketScanTime(result.Ticket.IdTicket);
                Console.WriteLine($"Ticket {result.Ticket.IdTicket} scan time updated");
            }
            return Ok(result);
        }
		[HttpPost("StartPriceUpdate")]
		public async Task<ActionResult> StartPriceUpdate()
		{
			var buyWeight = await _ticketLogic.InitiliazeBuyWeight(); //2
			var events = await _ticketLogic.CollectEventsData(); //3-4
			if (events.Value == null)
			{
				return BadRequest("Failed to retrieve events data");
			}
			var eventList = events.Value;
			foreach (var e in eventList)
			{
				var eventTickets = await _ticketLogic.FetchAllEventTickets(e.IdEvent); //5-6
				var eventSameCategorySectorPrice = await _ticketLogic.CollectSameCategoryEventSectorPrices(e.IdEvent);//7-8 
				var organiser = await _ticketLogic.GetOrganiserByEvent(e.FkOrganiseridUser);//9-10
				var task1 = Task.Run(() => {
                    var task1BW = buyWeight;
					var speedWeight = _ticketLogic.SoldEventTicketSpeed(e, eventTickets.Value);//11
					if (speedWeight < 1)
						task1BW = _ticketLogic.LowerBuyWeight(task1BW); //12
					else if (speedWeight > 10)
						task1BW = _ticketLogic.IncreaseBuyWeight(task1BW); //13

					var quantityWeight = _ticketLogic.RemainingEventTicketQuantity(e, eventTickets.Value);//14
					if (quantityWeight > 90)
						task1BW = _ticketLogic.LowerBuyWeight(task1BW); //15
					else if (quantityWeight < 20)
						task1BW = _ticketLogic.IncreaseBuyWeight(task1BW); //16

			        var monthsUntilEvent = _ticketLogic.RemainingWaitingTime(e);//17
					if (monthsUntilEvent >= 6)
						task1BW = _ticketLogic.LowerBuyWeight(task1BW); //18
					else if (monthsUntilEvent < 1)
						task1BW = _ticketLogic.IncreaseBuyWeight(task1BW); //19
                    return task1BW;
				});

				var task2 = Task.Run(() => {
					var mode = _ticketLogic.CalculateModeAndMultiply(eventSameCategorySectorPrice);//20
					return _ticketLogic.IncludeToWeight(buyWeight, mode);//21
				});

				var task3 = Task.Run(() => {
					double result = buyWeight;
					if (organiser.Rating < 3)
						result = _ticketLogic.LowerBuyWeight(result);//22
					else if (organiser.Rating > 8.5)
						result = _ticketLogic.IncreaseBuyWeight(result);//23
					if (organiser.FollowerCount < 1000)
						result = _ticketLogic.LowerBuyWeight(result);//24
					else if (organiser.FollowerCount > 100000)
						result = _ticketLogic.IncreaseBuyWeight(result);//25
					return result;
				});

				await Task.WhenAll(task1, task2, task3);
				var result1 = task1.Result-1;
				var result2 = task2.Result-1;
				var result3 = task3.Result-1;
				var final = (1+result1 + result2 + result3);
				await _ticketLogic.MultiplyWeightAndSectorPrices(e.IdEvent, final);//26
			}
			return Ok();
		}
	}
}
