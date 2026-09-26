namespace EcommerceAPI.Models.Order
{
    public class OrderTracking
    {
        public int TrackingId { get; set; }
        public int OrderId { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? TrackingNote { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
    }
}