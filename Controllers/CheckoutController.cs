using System.Security.Claims;
using Asp.Versioning;
using EcommerceAPI.DTOs.Checkout;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/checkout")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Customer")]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(
            ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        public async Task<IActionResult> CalculateCheckout(
            CheckoutRequestDto request,
            CancellationToken cancellationToken)
        {
            var customerId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var customerName = User.FindFirstValue(
                 ClaimTypes.Name)!;

            var result =
                await _checkoutService.CalculateCheckoutAsync(
                    customerId,
                    customerName,
                    request,
                    cancellationToken);

            return Ok(result);
        }
    }
}