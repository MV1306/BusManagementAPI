using BusManagementAPI.Data;
using BusManagementAPI.DTOs;
using BusManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly BusDbContext _context;

        public TicketsController(BusDbContext context)
        {
            _context = context;
        }

        [HttpPost("/CreateTicket")]
        public async Task<IActionResult> SaveTicket(TicketDTO tktIP)
        {
            var routeID = _context.BusRoutes.Where(x => x.RouteCode.ToLower() == tktIP.RouteCode.ToLower()).Select(x => x.RouteID).FirstOrDefault();

            if(routeID != Guid.Empty)
            {
                var ticket = new Ticket
                {
                    TicketID = Guid.NewGuid(),
                    RouteID = routeID,
                    FromStage = tktIP.FromStage,
                    ToStage = tktIP.ToStage,
                    BusType = tktIP.BusType,
                    StagesTravelled = tktIP.StagesTravelled,
                    Fare = tktIP.Fare,
                    UserName = tktIP.UserName,
                    MobileNo = tktIP.MobileNo,
                    Email = tktIP.Email,
                    IsActive = true,
                    BookingDate = DateTime.Now
                };

                _context.Tickets.Add(ticket);

                var sucess = await _context.SaveChangesAsync() > 0;
                if(sucess) 
                {
                    return Ok(ticket.TicketID);
                }

                return BadRequest("Unable to proceed with the booking. Please try again later...");
            }
            else
            {
                return BadRequest("Route Not Found");
            }
        }

        [HttpPost]
        public string Test()
        {
            return string.Empty;
        }
    }
}
