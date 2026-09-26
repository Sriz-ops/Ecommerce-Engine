using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterDto dto);

        Task<LoginResponseDto?> LoginAsync(LoginDto dto);

        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);

        Task LogoutAsync(string refreshToken);

    }
}