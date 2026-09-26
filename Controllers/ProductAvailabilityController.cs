using EcommerceAPI.DTO;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/availability")]
    [Authorize(Roles = "Admin")]
    public class ProductAvailabilityController : ControllerBase
    {
        private readonly IProductAvailabilityService _service;

        public ProductAvailabilityController(
            IProductAvailabilityService service)
        {
            _service = service;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductAvailabilityDto>>
            GetAvailability(int productId, CancellationToken cancellationToken)
        {
            var result =
                await _service.GetAvailabilityAsync(productId,
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("check")]
        public async Task<
            ActionResult<List<InventoryCheckResultDto>>>
            CheckInventory(
                [FromBody] List<InventoryCheckItemDto> items,
                CancellationToken cancellationToken)
        {
            var result =
                await _service.CheckInventoryAsync(items,cancellationToken);

            return Ok(result);
        }
    }
}