using System.Security.Claims;
using Asp.Versioning;
using EcommerceAPI.DTO;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/cart")]
    [ApiVersion("2.0")]
    [Authorize(Roles = "Customer")]
    public class CartV2Controller : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartV2Controller(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult> GetCart(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var cart = await _cartService.GetCartAsync(
                customerId,
                cancellationToken);

            return Ok(new
            {
                apiVersion = "v2",
                items = cart
            });
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(
            [FromBody] AddCartItemDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            await _cartService.AddItemAsync(
                customerId,
                dto,
                cancellationToken);

            return Ok(new
            {
                apiVersion = "v2",
                message = "Item added to cart."
            });
        }

        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateItem(
            int productId,
            [FromBody] UpdateCartItemDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var updated = await _cartService.UpdateItemAsync(
                customerId,
                productId,
                dto,
                cancellationToken);

            if (!updated)
                return NotFound();

            return Ok(new
            {
                apiVersion = "v2",
                message = "Cart item updated."
            });
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(
            int productId,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var removed = await _cartService.RemoveItemAsync(
                customerId,
                productId,
                cancellationToken);

            if (!removed)
                return NotFound();

            return Ok(new
            {
                apiVersion = "v2",
                message = "Item removed from cart."
            });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            await _cartService.ClearCartAsync(
                customerId,
                cancellationToken);

            return Ok(new
            {
                apiVersion = "v2",
                message = "Cart cleared."
            });
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetItemCount(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var count = await _cartService.GetItemCountAsync(
                customerId,
                cancellationToken);

            return Ok(new
            {
                apiVersion = "v2",
                itemCount = count
            });
        }

        [HttpGet("subtotal")]
        public async Task<ActionResult> GetSubtotal(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var subtotal = await _cartService.GetSubtotalAsync(
                customerId,
                cancellationToken);

            return Ok(new
            {
                apiVersion = "v2",
                subtotal
            });
        }

        private int GetCustomerId()
        {
            return int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);
        }
    }
}