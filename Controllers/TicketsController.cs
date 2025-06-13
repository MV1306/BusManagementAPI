using BusManagementAPI.Data;
using BusManagementAPI.DTOs;
using BusManagementAPI.Models;
using BusManagementAPI.Services;
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
        private readonly IEmailService _emailService;
        private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly Random random = new Random();

        public TicketsController(BusDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                    BookingRefId = t.BookingRefID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    Passengers = t.Passengers,
                    TotalFare = t.TotalFare,
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
        public async Task<ActionResult<GroupedTicketsResponseDTO>> GetTicketsByMobile(string mobileNo)
        {
            var redeemedTickets = await _context.Tickets
                .Include(t => t.BusRoute)
                .Where(t => t.MobileNo == mobileNo && t.IsActive == true && t.IsRedeemed == true)
                .Select(t => new TicketResponseDTO
                {
                    TicketID = t.TicketID,
                    BookingRefId = t.BookingRefID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    Passengers = t.Passengers,
                    TotalFare = t.TotalFare,
                    UserName = t.UserName,
                    MobileNo = t.MobileNo,
                    Email = t.Email,
                    IsActive = t.IsActive,
                    IsRedeemed = t.IsRedeemed,
                    IsCancelled = t.IsCancelled,
                    BookingDate = t.BookingDate,
                    RedeemedDate = t.RedeemedDate
                })
                .ToListAsync();

            var activeTickets = await _context.Tickets
                .Include(t => t.BusRoute)
                .Where(t => t.MobileNo == mobileNo && t.IsActive == true && t.IsCancelled == false && t.IsRedeemed == false)
                .Select(t => new TicketResponseDTO
                {
                    TicketID = t.TicketID,
                    BookingRefId = t.BookingRefID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    Passengers = t.Passengers,
                    TotalFare = t.TotalFare,
                    UserName = t.UserName,
                    MobileNo = t.MobileNo,
                    Email = t.Email,
                    IsActive = t.IsActive,
                    IsRedeemed = t.IsRedeemed,
                    IsCancelled = t.IsCancelled,
                    BookingDate = t.BookingDate,
                    RedeemedDate = t.RedeemedDate
                })
                .ToListAsync();

            var cancelledTickets = await _context.Tickets
                .Include(t => t.BusRoute)
                .Where(t => t.MobileNo == mobileNo && t.IsCancelled == true)
                .Select(t => new TicketResponseDTO
                {
                    TicketID = t.TicketID,
                    BookingRefId = t.BookingRefID,
                    RouteCode = t.BusRoute.RouteCode,
                    FromStage = t.FromStage,
                    ToStage = t.ToStage,
                    BusType = t.BusType,
                    StagesTravelled = t.StagesTravelled,
                    Fare = t.BaseFare,
                    Passengers = t.Passengers,
                    TotalFare = t.TotalFare,
                    UserName = t.UserName,
                    MobileNo = t.MobileNo,
                    Email = t.Email,
                    IsActive = t.IsActive,
                    IsRedeemed = t.IsRedeemed,
                    IsCancelled = t.IsCancelled,
                    BookingDate = t.BookingDate,
                    RedeemedDate = t.RedeemedDate
                })
                .ToListAsync();

            GroupedTicketsResponseDTO finalRes = new GroupedTicketsResponseDTO();
            finalRes.Active = activeTickets;
            finalRes.Redeemed = redeemedTickets;
            finalRes.Cancelled = cancelledTickets;

            //if (!finalRes.Any())
            //{
            //    return NotFound("No tickets found for this mobile number.");
            //}

            return Ok(finalRes);
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

            if (!string.IsNullOrEmpty(ticketDto.Email))
            {
                var subject = $"Booking ID: {ticket.BookingRefID} - Bus Ticket Booking Confirmation";
                var emailBody = $@"
                        <!DOCTYPE html>
                        <html lang='en'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Bus Ticket Confirmation</title>
                            <style>
                                body {{ font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; }}
                                .container {{ max-width: 600px; margin: 20px auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; background-color: #f9f9f9; }}
                                .header {{ background-color: #4361ee; color: white; padding: 15px; text-align: center; border-radius: 8px 8px 0 0; }}
                                .content {{ padding: 20px 0; }}
                                .footer {{ margin-top: 20px; font-size: 0.9em; color: #777; text-align: center; border-top: 1px solid #eee; padding-top: 10px; }}
                                .detail-row {{ display: flex; justify-content: space-between; padding: 5px 0; border-bottom: 1px dashed #eee; }}
                                .detail-row:last-child {{ border-bottom: none; }}
                                strong {{ color: #000; }}
                                .fare-total {{ font-size: 1.2em; font-weight: bold; text-align: right; margin-top: 15px; padding-top: 10px; border-top: 2px solid #4361ee; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h2>Bus Ticket Booking Confirmation</h2>
                                </div>
                                <div class='content'>
                                    <p>Dear {ticketDto.UserName ?? "Passenger"},</p>
                                    <p>Your bus ticket booking has been successfully confirmed!</p>
                                    <p>Here are your booking details:</p>
                                    <div class='detail-row'><span>Booking Reference ID:</span> <strong>{ticket.BookingRefID}</strong></div>
                                    <div class='detail-row'><span>Route:</span> <strong>{ticketDto.RouteCode}</strong></div>
                                    <div class='detail-row'><span>From:</span> <strong>{ticketDto.FromStage}</strong></div>
                                    <div class='detail-row'><span>To:</span> <strong>{ticketDto.ToStage}</strong></div>
                                    <div class='detail-row'><span>Bus Type:</span> <strong>{ticketDto.BusType}</strong></div>
                                    <div class='detail-row'><span>Number of Passengers:</span> <strong>{ticketDto.Passengers}</strong></div>
                                    <div class='detail-row'><span>Passenger Name:</span> <strong>{ticketDto.UserName}</strong></div>
                                    <div class='detail-row'><span>Mobile Number:</span> <strong>{ticketDto.MobileNo}</strong></div>
                                    {(!string.IsNullOrEmpty(ticketDto.Email) ? $"<div class='detail-row'><span>Email Address:</span> <strong>{ticketDto.Email}</strong></div>" : "")}

                                    <p class='fare-total'>Total Paid: ₹{ticket.TotalFare:F2}</p>

                                    <p>Please keep this email for your reference. We wish you a pleasant journey!</p>
                                    <p>Sincerely,<br>The Bus Booking Team</p>
                                </div>
                                <div class='footer'>
                                    This is an automated email, please do not reply.
                                </div>
                            </div>
                        </body>
                        </html>";

                await _emailService.SendEmailAsync(ticketDto.Email, subject, emailBody);
            }
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
        public async Task<IActionResult> CancelTicket(string id)
        {
            var ticket = await _context.Tickets.Where(x => x.BookingRefID == id).FirstOrDefaultAsync();
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