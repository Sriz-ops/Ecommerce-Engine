using System.Security.Claims;
using Asp.Versioning;
using EcommerceAPI.DTOs;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/products")]
    [ApiVersion("2.0")]
    [Authorize(Roles = "Customer")]
    public class ProductsV2Controller : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsV2Controller(
            IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult> GetProductsV2(
            CancellationToken cancellationToken)
        {
            var customerId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var products =
                await _productService.GetAllAsync(customerId,
                    cancellationToken);

            var response = products.Select(product => new
            {
                product.ProductId,
                product.Name,
                product.Description,
                product.CategoryId,
                product.CategoryName,
                product.BrandId,
                product.BrandName,
                product.Price,
                product.Rating,
                ApiVersion = "v2"
            });

            return Ok(response);
        }
    }
}