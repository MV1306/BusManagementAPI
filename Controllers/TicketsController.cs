using BusManagementAPI.Data;
using BusManagementAPI.DTOs;
using BusManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly BusDbContext _context;
        private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly Random random = new Random();

        public TicketsController(BusDbContext context)
        {
            _context = context;
        }

        // GET: api/Tickets - Get all tickets
        [HttpGet("/GetAllTickets")]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetTickets()
        {
            var tickets = await _context.Tickets
                .Include(t => t.BusRoute)
                .Select(t => new TicketResponseDTO
                {
                    TicketID = t.TicketID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    UserName = t.UserName,
                    MobileNo = t.MobileNo,
                    Email = t.Email,
                    IsActive = t.IsActive,
                    IsRedeemed = t.IsRedeemed,
                    BookingDate = t.BookingDate,
                    RedeemedDate = t.RedeemedDate
                })
                .ToListAsync();

            return Ok(tickets);
        }

        // GET: api/Tickets/{id} - Get ticket by ID
        [HttpGet("/GetTicketByID/{id}")]
        public async Task<ActionResult<TicketResponseDTO>> GetTicket(Guid id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.BusRoute)
                .Where(t => t.TicketID == id)
                .Select(t => new TicketResponseDTO
                {
                    TicketID = t.TicketID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    UserName = t.UserName,
                    MobileNo = t.MobileNo,
                    Email = t.Email,
                    IsActive = t.IsActive,
                    IsRedeemed = t.IsRedeemed,
                    BookingDate = t.BookingDate,
                    RedeemedDate = t.RedeemedDate
                })
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        //GET: api/Tickets/mobile/{mobileNo}
        [HttpGet("/GetTicketByMobileNo/{mobileNo}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDTO>>> GetTicketsByMobile(string mobileNo)
        {
            var tickets = await _context.Tickets
                .Include(t => t.BusRoute)
                .Where(t => t.MobileNo == mobileNo)
                .Select(t => new TicketResponseDTO
                {
                    TicketID = t.TicketID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    UserName = t.UserName,
                    MobileNo = t.MobileNo,
                    Email = t.Email,
                    IsActive = t.IsActive,
                    IsRedeemed = t.IsRedeemed,
                    BookingDate = t.BookingDate,
                    RedeemedDate = t.RedeemedDate
                })
                .ToListAsync();

            //if (!tickets.Any())
            //{
            //    return Ok("No tickets found for this mobile number.");
            //}

            return Ok(tickets);
        }

        // POST: api/Tickets - Purchase a new ticket
        [HttpPost("/BuyTicket")]
        public async Task<ActionResult<TicketResponseDTO>> PurchaseTicket([FromBody] TicketRequestDTO ticketDto)
        {
            // Validate the route exists
            var route = await _context.BusRoutes.Include(x => x.BusRouteStages).FirstOrDefaultAsync(r => r.RouteCode == ticketDto.RouteCode);
            if (route == null)
            {
                return BadRequest("Invalid route code.");
            }

            var stages = route.BusRouteStages.Where(s => s.IsActive).ToList();

            var start = stages.Where(s => s.StageName.ToLower() == ticketDto.FromStage.ToLower()).FirstOrDefault();
            var end = stages.Where(s => s.StageName.ToLower() == ticketDto.ToStage.ToLower()).FirstOrDefault();

            int stageDifference = Math.Abs(end.StageOrder - start.StageOrder);

            // Calculate fare based on stages travelled (you might have your own logic here)
            decimal fare = CalculateFare(stageDifference, ticketDto.BusType);

            string BookingId = GenerateAlphanumericCode();            

            // Create new ticket
            var ticket = new Ticket
            {
                TicketID = Guid.NewGuid(),
                BookingRefID = BookingId,
                RouteID = route.RouteID,
                FromStage = ticketDto.FromStage,
                ToStage = ticketDto.ToStage,
                BusType = ticketDto.BusType,
                StagesTravelled = stageDifference,
                BaseFare = fare,
                Passengers = ticketDto.Passengers,
                TotalFare = fare * ticketDto.Passengers,
                UserName = ticketDto.UserName,
                MobileNo = ticketDto.MobileNo,
                Email = ticketDto.Email,
                IsActive = true,
                IsRedeemed = false,
                BookingDate = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return Ok("Booking Successful. Booking Reference ID - " + ticket.BookingRefID);
        }

        // PUT: api/Tickets/{id}/redeem - Redeem a ticket
        [HttpPut("/RedeemTicket/{refID}")]
        public async Task<IActionResult> RedeemTicket(string refID)
        {
            var ticket = await _context.Tickets.Where(x => x.BookingRefID == refID).FirstOrDefaultAsync();
            if (ticket == null)
            {
                return NotFound();
            }

            if (!ticket.IsActive)
            {
                return Ok("Ticket is not active.");
            }

            if (ticket.IsRedeemed)
            {
                return Ok("Ticket is already redeemed.");
            }

            if (ticket.IsCancelled)
            {
                return Ok("Ticket is cancelled.");
            }

            ticket.IsRedeemed = true;
            ticket.RedeemedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok("Ticket has been successfully redeemed.");
        }

        // PUT: api/Tickets/{id}/cancel - Cancel a ticket
        [HttpPut("/CancelTicket/{id}")]
        public async Task<IActionResult> CancelTicket(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (!ticket.IsActive)
            {
                return Ok("Ticket is already cancelled.");
            }

            if (ticket.IsRedeemed)
            {
                return Ok("Cannot cancel a redeemed ticket.");
            }

            if (ticket.IsCancelled)
            {
                return Ok("Ticket is already cancelled.");
            }

            ticket.IsCancelled = true;
            await _context.SaveChangesAsync();

            return Ok("Ticket Cancelled Successfully");
        }

        private decimal CalculateFare(int stagesTravelled, string busType)
        {
            var fare = _context.FareMasters_New.Where(x => x.BusType == busType && x.Stage == stagesTravelled)
                .Select(x => x.StageFare).FirstOrDefault();

            return fare;
        }

        public static string GenerateAlphanumericCode()
        {
            var sb = new StringBuilder(10);
            for (int i = 0; i < 10; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }
}