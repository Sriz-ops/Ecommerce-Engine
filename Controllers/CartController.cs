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
    [ApiVersion("1.0")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [HttpGet]

        public async Task<ActionResult<List<CartItemDto>>> GetCart(CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();
            var cart =
                await _cartService.GetCartAsync(customerId, cancellationToken);

            return Ok(cart);
        }

        // POST: api/cart/items
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
                message = "Item added to cart."
            });
        }

        // PUT: api/cart/items
        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateItem(
            int productId,
            [FromBody] UpdateCartItemDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();
            var updated =
                await _cartService.UpdateItemAsync(
                    customerId,
                    productId,
                    dto,
                    cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/cart/items
        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveItem(
            int productId, CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();
            var removed =
                await _cartService.RemoveItemAsync(
                    customerId,
                    productId,
                    cancellationToken);

            if (!removed)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();
            await _cartService.ClearCartAsync(customerId, cancellationToken);

            return NoContent();
        }

        // GET: api/cart/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetItemCount(CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();
            var count =
                await _cartService.GetItemCountAsync(customerId, cancellationToken);

            return Ok(new
            {
                itemCount = count
            });
        }

        // GET: api/cart/subtotal?customerId=1
        [HttpGet("subtotal")]
        public async Task<ActionResult<decimal>> GetSubtotal(CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();
            var subtotal =
                await _cartService.GetSubtotalAsync(customerId, cancellationToken);

            return Ok(new
            {
                subtotal
            });
        }
        private int GetCustomerId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}