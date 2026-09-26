using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductPricesController : ControllerBase
    {
        private readonly IProductPriceService _service;

        public ProductPricesController(
            IProductPriceService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductPriceDto>> GetById(int id)
        {
            var price = await _service.GetByIdAsync(id);

            if (price == null)
            {
                return NotFound();
            }

            return Ok(price);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<ProductPriceDto>>
            GetCurrentPrice(int productId)
        {
            var price =
                await _service.GetCurrentPriceByProductIdAsync(productId);

            if (price == null)
            {
                return NotFound();
            }

            return Ok(price);
        }

        [HttpPost]
        public async Task<ActionResult> Create(
            CreateProductPriceDto dto)
        {
            var id = await _service.AddAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                new { productPriceId = id });
        }
    }
}