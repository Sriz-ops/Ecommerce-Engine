namespace EcommerceAPI.Models.Auth
{
    public class RefreshTokenSession
    {
        public int CustomerId { get; set; }

        public DateTime SessionExpiresAt { get; set; }

        public DateTime RefreshTokenExpiresAt { get; set; }

        public bool Revoked { get; set; }
    }
}