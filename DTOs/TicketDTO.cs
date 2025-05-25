using BusManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusManagementAPI.DTOs
{
    public class TicketDTO
    {
        public string RouteCode { get; set; }
        public string FromStage { get; set; }
        public string ToStage { get; set; }
        public string BusType { get; set; }
        public int StagesTravelled { get; set; }
        public decimal Fare { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
    }
}
