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

        public int StageOrder { get; set; } 

        public double DistanceFromStart { get; set; } 

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;

        // Navigation Property
        public Busroute? Busroute { get; set; }
    }
}
