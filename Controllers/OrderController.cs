using System.Security.Claims;
using Asp.Versioning;
using EcommerceAPI.Models.Order;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/orders")]
    [ApiVersion("1.0")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            Order order,
            CancellationToken cancellationToken)
        {
            var customerId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            order.CustomerId = customerId;
            order.CreatedBy = customerId;
            order.OrderStatus = "Pending";

            var orderId =
                await _orderService.CreateOrderAsync(
                    order,
                    cancellationToken);

            return Ok(new
            {
                OrderId = orderId,
                Message = "Order created successfully."
            });
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(
    int orderId,
    CancellationToken cancellationToken)
        {
            var customerId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var order =
                await _orderService.GetOrderDetailsAsync(
                    orderId,
                    cancellationToken);

            if (order == null)
            {
                return NotFound(new
                {
                    Message = "Order not found."
                });
            }

            if (order.CustomerId != customerId)
            {
                return Forbid();
            }

            return Ok(order);
        }

        [HttpPost("{orderId}/reorder")]
        public async Task<IActionResult> Reorder(
    int orderId,
    CancellationToken cancellationToken)
        {
            var customerId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            await _orderService.ReorderAsync(
                customerId,
                orderId,
                cancellationToken);

            return Ok(new
            {
                Message = "Order items added to cart successfully."
            });
        }

        [HttpGet("{orderId}/tracking")]
        public async Task<IActionResult> GetTracking(
    int orderId,
    CancellationToken cancellationToken)
        {
            var customerId =
                int.Parse(
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier)!);

            var tracking =
                await _orderService.GetOrderTrackingAsync(
                    orderId,
                    customerId,
                    cancellationToken);

            return Ok(tracking);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory(
     [FromQuery] string? status,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10,
     CancellationToken cancellationToken = default)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            var customerId = int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);

            var result =
                await _orderService.GetOrderHistoryAsync(
                    customerId,
                    status,
                    page,
                    pageSize,
                    cancellationToken);

            var totalPages = pageSize > 0
                ? (int)Math.Ceiling(
                    (double)result.TotalRecords / pageSize)
                : 0;

            return Ok(new
            {
                result.Orders,
                Page = page,
                PageSize = pageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = totalPages
            });
        }

    }
}