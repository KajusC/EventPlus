using EventPlus.Server.Application.Authentication;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EventPlus.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRoleBasedAuthService _authService;

        public UserController(IRoleBasedAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.AuthenticateAsync(model.Username, model.Password);
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(new
            {
                Id = result.UserId,
                Username = result.Username,
                Name = result.Name,
                Surname = result.Surname,
                Role = result.Role,
                Token = result.Token
            });
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterUserAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                Id = result.UserId,
                Username = result.Username,
                Name = result.Name,
                Surname = result.Surname,
                Role = result.Role,
                Token = result.Token
            });
        }

        [HttpPost("register/organiser")]
        public async Task<IActionResult> RegisterOrganiser([FromBody] OrganiserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterOrganiserAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                Id = result.UserId,
                Username = result.Username,
                Name = result.Name,
                Surname = result.Surname,
                Role = result.Role,
                Token = result.Token
            });
        }

        [HttpPost("register/administrator")]
        [Authorize(Roles = "Administrator")] // Only administrators can create other administrators
        public async Task<IActionResult> RegisterAdministrator([FromBody] AdministratorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAdministratorAsync(model);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new
            {
                Id = result.UserId,
                Username = result.Username,
                Name = result.Name,
                Surname = result.Surname,
                Role = result.Role,
                Token = result.Token
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            // Get the user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId) || roleClaim == null)
            {
                return Unauthorized();
            }

            var success = await _authService.ChangePasswordAsync(userId, roleClaim.Value, model.CurrentPassword, model.NewPassword);
            if (!success)
            {
                return BadRequest(new { message = "Failed to change password. Check that your current password is correct." });
            }

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("signout")]
        [Authorize]
        public async Task<IActionResult> SignOut()
        {
            int userId;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                return Unauthorized();
            }

            var result = await _authService.SignOutAsync(userId);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "User signed out successfully" });
        }
    }
}