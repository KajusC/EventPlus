using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackLogic _feedbackLogic;

        public FeedbackController(IFeedbackLogic feedbackLogic)
        {
            _feedbackLogic = feedbackLogic;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            var feedbacks = await _feedbackLogic.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var feedback = await _feedbackLogic.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return Ok(feedback);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackViewModel feedback)
        {
            if (feedback == null)
            {
                return BadRequest("Feedback cannot be null.");
            }
            var result = await _feedbackLogic.CreateFeedbackAsync(feedback);
            if (result)
            {
                return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.IdFeedback }, feedback);
            }
            return BadRequest("Failed to create feedback.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var result = await _feedbackLogic.DeleteFeedbackAsync(id);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("event/{eventId}")]
        public async Task<IActionResult> DeleteEventFeedbacks(int eventId)
        {
            var result = await _feedbackLogic.DeleteEventFeedbacks(eventId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeedback([FromBody] FeedbackViewModel feedback)
        {
            if (feedback == null)
            {
                return BadRequest("Feedback cannot be null.");
            }
            var result = await _feedbackLogic.UpdateFeedbackAsync(feedback);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
