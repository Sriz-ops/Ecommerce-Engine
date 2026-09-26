namespace EcommerceAPI.DTOs.Address
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public int CustomerId { get; set; }

        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }

        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
    }
}