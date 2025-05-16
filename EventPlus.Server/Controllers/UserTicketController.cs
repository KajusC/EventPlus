using eventplus.models.Domain.Tickets;
using eventplus.models.Infrastructure.context;
using EventPlus.Server.Application.Authentication;
using EventPlus.Server.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventPlus.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserTicketController : ControllerBase
	{
		private readonly EventPlusContext _context;

		public UserTicketController(EventPlusContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<ActionResult> CreateUserTicket([FromBody] UserTicket userTicket)
		{
			Console.WriteLine($"Gauta: {JsonSerializer.Serialize(userTicket)}");

			if (userTicket == null || userTicket.FkUseridUser == 0 || userTicket.FkTicketidTicket == 0)
			{
				return BadRequest("Neteisingi duomenys");
			}

			_context.UserTickets.Add(userTicket);
			await _context.SaveChangesAsync();
			return Ok(userTicket);
		}

		[HttpGet("{userId}")]
		public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByUserId(int userId)
		{
			if (userId <= 0)
			{
				return BadRequest("Invalid user ID");
			}

			var userTickets = await _context.UserTickets
				.Where(ut => ut.FkUseridUser == userId)
				.Join(
					_context.Tickets,
					userTicket => userTicket.FkTicketidTicket,
					ticket => ticket.IdTicket,
					(userTicket, ticket) => ticket
				)
				.ToListAsync();

			if (userTickets == null || !userTickets.Any())
			{
				return NotFound($"No tickets found for user with ID {userId}");
			}

			return Ok(userTickets);
		}
	}
}