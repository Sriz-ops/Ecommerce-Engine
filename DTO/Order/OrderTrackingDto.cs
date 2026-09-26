namespace EcommerceAPI.DTOs.Order
{
    public class OrderTrackingDto
    {
        public int TrackingId { get; set; }
        public int OrderId { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? TrackingNote { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}