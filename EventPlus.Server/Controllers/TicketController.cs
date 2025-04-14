using eventplus.models.Entities;
using EventPlus.Server.DTO;
using EventPlus.Server.Logic.Interface;
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
        public async Task<ActionResult<List<TicketDTO>>> GetAllTickets()
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
        public async Task<ActionResult<bool>> CreateTicket([FromBody] TicketDTO ticketDTO)
        {
            if (ticketDTO == null)
            {
                return BadRequest("Ticket cannot be null");
            }
            var result = await _ticketLogic.CreateTicketAsync(ticketDTO);
            return CreatedAtAction(nameof(GetTicketById), new { id = ticketDTO.IdTicket }, result);
        }
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateTicket([FromBody] TicketDTO ticketDTO)
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
    }
}
