using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusManagementAPI.Models
{
    public class Busroute
    {
        [Key]
        public Guid RouteID { get; set; }
        public string RouteCode { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(100)")]
        public string StartingPoint { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(100)")]
        public string EndingPoint { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public ICollection<BusRouteStage>? BusRouteStages { get; set; }
    }
}