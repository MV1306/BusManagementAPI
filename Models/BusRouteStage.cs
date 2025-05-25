using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusManagementAPI.Models
{
    public class BusRouteStage
    {
        [Key]
        public Guid StageID { get; set; }

        [ForeignKey("Busroute")]
        public Guid RouteID { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string StageName { get; set; } = string.Empty;

        public int StageOrder { get; set; } // Used to define the order of the stage in the route

        public double DistanceFromStart { get; set; } // Optional: useful for fare calculation

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        // Navigation Property
        public Busroute? Busroute { get; set; }
    }
}
