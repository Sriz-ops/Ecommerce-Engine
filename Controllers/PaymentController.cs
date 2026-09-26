using EcommerceAPI.DTOs.Payment;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize(Roles = "Customer")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("mock")]
        public async Task<IActionResult> ProcessMockPayment(
            MockPaymentRequestDto request,
            CancellationToken cancellationToken)
        {
            var customerId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var result =
                await _paymentService.ProcessPaymentAsync(
                    customerId,
                    request,
                    cancellationToken);

            return Ok(result);
        }
    }
}