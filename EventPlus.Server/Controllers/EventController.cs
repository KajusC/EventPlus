using EventPlus.Server.Application.Events.Handler;
using EventPlus.Server.Application.Events.ViewModel;
using EventPlus.Server.Application.Feedbacks.Handler;
using EventPlus.Server.Application.Sectors.Handler;
using EventPlus.Server.Application.Tickets.Handler;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventLogic _eventLogic;
        private readonly ITicketLogic _ticketLogic;
        private readonly IFeedbackLogic _feedbackLogic;
        private readonly ISectorLogic _sectorLogic;
        public EventController(IEventLogic eventLogic, ITicketLogic ticketLogic, IFeedbackLogic feedbackLogic, ISectorLogic sectorLogic)
        {
            _eventLogic = eventLogic;
            _ticketLogic = ticketLogic;
            _feedbackLogic = feedbackLogic;
            _sectorLogic = sectorLogic;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventViewModel>>> GetAllEvents()
        {
            var events = await _eventLogic.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventViewModel>> GetEventById(int id)
        {
            var eventEntity = await _eventLogic.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }
            return Ok(eventEntity);
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateEvent([FromBody] EventViewModel eventEntity)
        {
            if (eventEntity == null)
            {
                return BadRequest("Event cannot be null");
            }
            var result = await _eventLogic.CreateEventAsync(eventEntity);
            return CreatedAtAction(nameof(GetEventById), new { id = eventEntity.IdEvent }, result);
        }

        [HttpPut]
        public async Task<ActionResult<bool>> UpdateEvent([FromBody] EventViewModel eventEntity)
        {
            if (eventEntity == null)
            {
                return BadRequest("Event cannot be null");
            }
            var result = await _eventLogic.UpdateEventAsync(eventEntity);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteEvent(int id)
        {

            // Check if the event has tickets before deleting
            var hasTickets = await _ticketLogic.IfEventHasTickets(id);
            if (hasTickets)
            {
                return BadRequest("Cannot delete event with associated tickets.");
            }

            // If the event has feedback, delete them first
            var feedbackDeleted = await _feedbackLogic.DeleteEventFeedbacks(id);
            Console.WriteLine($"Feedback deleted: {feedbackDeleted}"); // should be logger

            // delete sectors
            var sectorsDeleted = await _sectorLogic.DeleteEventSectors(id);
            Console.WriteLine($"Sectors deleted: {sectorsDeleted}"); // should be logger

            var result = await _eventLogic.DeleteEventAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();

        }
    }
}
