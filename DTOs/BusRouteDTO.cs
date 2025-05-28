using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusManagementAPI.DTOs
{
    public class BusRouteDTO
    {
        public string RouteCode { get; set; } = string.Empty;
        public string StartPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public List<BusRouteStagesDTO> BusStages { get; set; }
    }

    public class FareMasterDTO
    {
        public string BusType { get; set; } = string.Empty;
        public decimal BaseFare { get; set; }
        public decimal FarePerStage { get; set; }
    }

    public class BusRouteStagesDTO
    {
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; } = 0;
        public int DistanceFromStart { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        public double Longitude { get; set; } = 0;
    }

    public class BusRouteStagesDisplayDTO
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public List<BusRouteStagesDTO> Stages { get; set; }
    }
}