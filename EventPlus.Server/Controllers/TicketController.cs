using eventplus.models.Domain.Tickets;
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
        public TicketController(ITicketLogic ticketLogic)
        {
            _ticketLogic = ticketLogic;
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
    }
}
