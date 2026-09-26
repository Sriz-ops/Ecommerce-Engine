using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Auth;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IMemoryCache _cache;

        public AuthService(
            IAuthRepository repository,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IMemoryCache cache)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
        }

        public async Task<int> RegisterAsync(RegisterDto dto)
        {
            var existingCustomer =
                await _repository.GetByEmailAsync(dto.Email);

            if (existingCustomer != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    dto.Password),
                Role = "Customer"
            };

            var customerId =
                await _repository.CreateCustomerAsync(customer);

            _logger.LogInformation(
                "Customer {CustomerId} registered successfully",
                customerId);

            return customerId;
        }

        public async Task<LoginResponseDto?> LoginAsync(
     LoginDto dto)
        {
            var customer =
                await _repository.GetByEmailAsync(dto.Email);

            if (customer == null)
            {
                _logger.LogWarning(
                    "Login failed for email {Email}",
                    dto.Email);

                return null;
            }

            var passwordValid =
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    customer.PasswordHash);

            if (!passwordValid)
            {
                _logger.LogWarning(
                    "Invalid password for email {Email}",
                    dto.Email);

                return null;
            }

            var accessToken =
                GenerateToken(customer);

            //generating new ref token
            var refreshToken =
                Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;

            var sessionExpiresAt =
                now.AddDays(14);

            var refreshTokenExpiresAt =
                now.AddDays(7);

            var session =
                new RefreshTokenSession
                {
                    CustomerId = customer.CustomerId,

                    SessionExpiresAt =
                        sessionExpiresAt,

                    RefreshTokenExpiresAt =
                        refreshTokenExpiresAt,

                    Revoked = false
                };

            _cache.Set(
                refreshToken,
                session,
                refreshTokenExpiresAt);

            _logger.LogInformation(
                "Customer {CustomerId} logged in. Session expires at {SessionExpiry}",
                customer.CustomerId,
                sessionExpiresAt);

            return new LoginResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Role = customer.Role
            };
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(
     string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            if (!_cache.TryGetValue(
                    refreshToken,
                    out RefreshTokenSession? session))
            {
                return null;
            }

            if (session == null || session.Revoked)
            {
                return null;
            }

            var now = DateTime.UtcNow;

            // Absolute 14-day session expiry
            if (now >= session.SessionExpiresAt)
            {
                _cache.Remove(refreshToken);

                _logger.LogWarning(
                    "Refresh rejected because absolute session expired for Customer {CustomerId}",
                    session.CustomerId);

                return null;
            }

            // Current refresh token expiry
            if (now >= session.RefreshTokenExpiresAt)
            {
                _cache.Remove(refreshToken);

                _logger.LogWarning(
                    "Refresh rejected because refresh token expired for Customer {CustomerId}",
                    session.CustomerId);

                return null;
            }

            var customer =
                await _repository.GetByIdAsync(
                    session.CustomerId);

            if (customer == null)
            {
                _cache.Remove(refreshToken);

                return null;
            }

            var newAccessToken =
                GenerateToken(customer);

            var newRefreshToken =
                Guid.NewGuid().ToString();

            // Rotate refresh token
            _cache.Remove(refreshToken);

            // New token gets 7 days OR whatever remains
            // until the absolute 14-day session expiry.
            var newRefreshTokenExpiry =
                now.AddDays(7) < session.SessionExpiresAt
                    ? now.AddDays(7)
                    : session.SessionExpiresAt;

            var newSession =
                new RefreshTokenSession
                {
                    CustomerId =
                        session.CustomerId,

                    // IMPORTANT:
                    // Keep the ORIGINAL session expiry.
                    SessionExpiresAt =
                        session.SessionExpiresAt,

                    RefreshTokenExpiresAt =
                        newRefreshTokenExpiry,

                    Revoked = false
                };

            _cache.Set(
                newRefreshToken,
                newSession,
                newRefreshTokenExpiry);

            _logger.LogInformation(
                "Refresh token rotated for Customer {CustomerId}. Session expires at {SessionExpiry}",
                customer.CustomerId,
                session.SessionExpiresAt);

            return new LoginResponseDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                Role = customer.Role
            };
        }


        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }

            if (_cache.TryGetValue(
                    refreshToken,
                    out RefreshTokenSession? session))
            {
                if (session != null)
                {
                    session.Revoked = true;
                }
            }

            _cache.Remove(refreshToken);

            _logger.LogInformation(
                "Refresh token revoked during logout");
        }
        private string GenerateToken(Customer customer)
        {
            var jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT key is not configured.");

            var issuer =
                _configuration["Jwt:Issuer"];

            var audience =
                _configuration["Jwt:Audience"];

            var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                customer.CustomerId.ToString()),

            new Claim(
                ClaimTypes.Name,
                customer.Name),

            new Claim(
                ClaimTypes.Email,
                customer.Email),

            new Claim(
                ClaimTypes.Role,
                customer.Role)
        };

                    if (customer.Role == "Admin")
                    {
                        claims.Add(
                            new Claim("CanManageProducts", "true"));

                        claims.Add(
                            new Claim("CanManageOrders", "true"));
                    }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}