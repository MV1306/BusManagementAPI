using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusManagementAPI.Models
{
    public class Ticket
    {
        [Key]
        public Guid TicketID { get; set; }

        [Required]
        public string BookingRefID { get; set; }

        [Required]
        public Guid RouteID { get; set; }

        [ForeignKey("RouteID")]
        public virtual Busroute BusRoute { get; set; }

        [Required]
        public string FromStage { get; set; }

        [Required]
        public string ToStage { get; set; }

        [Required]
        public string BusType { get; set; }

        [Required]
        public int StagesTravelled { get; set; }

        [Precision(10, 2)]
        [Range(0, double.MaxValue)]
        public decimal BaseFare { get; set; }

        public int Passengers { get; set; }

        [Precision(10, 2)]
        [Range(0, double.MaxValue)]
        public decimal TotalFare { get; set; }

        [Required]
        public string UserName { get; set; }

        [Phone]
        public string MobileNo { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsRedeemed { get; set; } = false;

        public bool IsCancelled { get; set; } = false;

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public DateTime? RedeemedDate { get; set; }
    }
}
