using EventPlus.Server.Application.IHandlers;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User, Administrator, Organiser")]
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
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetFeedbacksByEventId(int eventId)
        {
            var feedbacks = await _feedbackLogic.GetFeedbacksByEventIdAsync(eventId);
            if (feedbacks == null || !feedbacks.Any())
            {
                return NotFound("No feedbacks found for this event.");
            }
            return Ok(feedbacks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackViewModel feedback)
        {
            if (feedback == null)
                return BadRequest("Feedback cannot be null.");

            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "sub" ||
                c.Type == "userId" ||
                c.Type == "nameid" ||
                c.Type == System.Security.Claims.ClaimTypes.NameIdentifier
            );
            var roleClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "role" ||
                c.Type == System.Security.Claims.ClaimTypes.Role
            );

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId) || roleClaim == null)
                return Unauthorized("User ID or role not found in token.");

            var result = await _feedbackLogic.CreateFeedbackAsync(feedback, userId, roleClaim.Value);
            if (result)
                return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.IdFeedback }, feedback);

            return BadRequest("Failed to create feedback.");
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteEventFeedbacks(int eventId)
        {
            var result = await _feedbackLogic.DeleteEventFeedbacks(eventId);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
