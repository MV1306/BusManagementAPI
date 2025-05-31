using System.ComponentModel.DataAnnotations;

namespace BusManagementAPI.Models
{
    public class StageCoordinate
    {
        [Key]
        public Guid CoordinateId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
