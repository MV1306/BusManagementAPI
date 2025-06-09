namespace BusManagementAPI.DTOs
{
    public class TicketResponseDTO
    {
        public Guid TicketID { get; set; }
        public string RouteCode { get; set; }
        public string FromStage { get; set; }
        public string ToStage { get; set; }
        public string BusType { get; set; }
        public int StagesTravelled { get; set; }
        public decimal Fare { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsRedeemed { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? RedeemedDate { get; set; }
    }

    public class TicketRequestDTO
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
