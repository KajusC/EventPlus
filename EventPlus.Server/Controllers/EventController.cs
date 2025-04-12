using EventPlus.Server.DTO;
using EventPlus.Server.Logic.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventLogic _eventLogic;
        public EventController(IEventLogic eventLogic)
        {
            _eventLogic = eventLogic;
        }
        [HttpGet]
        public async Task<ActionResult<List<EventDTO>>> GetAllEvents()
        {
            var events = await _eventLogic.GetAllEventsAsync();
            return Ok(events);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int id)
        {
            var eventEntity = await _eventLogic.GetEventByIdAsync(id);
            if (eventEntity == null)
            {
                return NotFound();
            }
            return Ok(eventEntity);
        }
        [HttpPost]
        public async Task<ActionResult<bool>> CreateEvent([FromBody] EventDTO eventEntity)
        {
            if (eventEntity == null)
            {
                return BadRequest("Event cannot be null");
            }
            var result = await _eventLogic.CreateEventAsync(eventEntity);
            return CreatedAtAction(nameof(GetEventById), new { id = eventEntity.IdEvent }, result);
        }
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateEvent([FromBody] EventDTO eventEntity)
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
            var result = await _eventLogic.DeleteEventAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
