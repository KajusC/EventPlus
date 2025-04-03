using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EventPlus.Server.Event.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        // In-memory list to simulate a database
        private static List<Event> events = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Description = "Description 1" },
            new Event { Id = 2, Name = "Event 2", Description = "Description 2" }
        };

        // GET: api/Event
        [HttpGet]
        public ActionResult<IEnumerable<Event>> GetEvents()
        {
            return Ok(events);
        }

        // GET: api/Event/5
        [HttpGet("{id}")]
        public ActionResult<Event> GetEvent(int id)
        {
            var eventItem = events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return Ok(eventItem);
        }

        // POST: api/Event
        [HttpPost]
        public ActionResult<Event> PostEvent(Event newEvent)
        {
            newEvent.Id = events.Count + 1;
            events.Add(newEvent);
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, newEvent);
        }

        // PUT: api/Event/5
        [HttpPut("{id}")]
        public IActionResult PutEvent(int id, Event updatedEvent)
        {
            var eventItem = events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            eventItem.Name = updatedEvent.Name;
            eventItem.Description = updatedEvent.Description;

            return NoContent();
        }

        // DELETE: api/Event/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var eventItem = events.FirstOrDefault(e => e.Id == id);
            if (eventItem == null)
            {
                return NotFound();
            }

            events.Remove(eventItem);
            return NoContent();
        }
    }

    // Event model
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
