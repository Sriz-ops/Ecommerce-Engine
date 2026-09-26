using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Asp.Versioning;
using EcommerceAPI.DTOs;
using EcommerceAPI.DTOs.Inventory;
using EcommerceAPI.Models;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Get all products - public
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetProducts(
            CancellationToken cancellationToken)
        {
            var products =
                await _productService.GetAllAsync(
                    0,
                    cancellationToken);

            // Convert product list to JSON
            var json =
                JsonSerializer.Serialize(products);

            // Generate SHA256 hash
            var hash =
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(json));

            // Convert hash to hexadecimal string
            var etagValue =
                Convert.ToHexString(hash);

            // HTTP ETag should be quoted
            var etag = $"\"{etagValue}\"";

            // Check client's IfNoneMatch header
            var clientEtag =
                Request.Headers.IfNoneMatch.ToString();

            if (clientEtag == etag)
            {
                return StatusCode(
                    StatusCodes.Status304NotModified);
            }

            // Send ETag to client
            Response.Headers.ETag = etag;

            return Ok(products);
        }

        // Get single product - public
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(
            int id,
            CancellationToken cancellationToken)
        {
            var product =
                await _productService.GetByIdAsync(
                    id,
                    cancellationToken);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // Search products - public
        [HttpGet("search")]
        public async Task<ActionResult<List<ProductPagedResponseDto>>> SearchProducts(
            [FromQuery] ProductQueryDto query,
            CancellationToken cancellationToken)
        {
            var products =
                await _productService.SearchAsync(
                    query,
                    cancellationToken);

            return Ok(products);
        }

        // Create product - Admin only
        [Authorize(Policy = "CanManageProducts")]
        [HttpPost]
        public async Task<IActionResult> Create(
            Product product,
            CancellationToken cancellationToken)
        {
            var id =
                await _productService.CreateAsync(
                    product,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetProduct),
                new { id },
                new { ProductId = id });
        }

        // Update product - Admin only
        [Authorize(Policy = "CanManageProducts")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            Product product,
            CancellationToken cancellationToken)
        {
            var updated =
                await _productService.UpdateAsync(
                    id,
                    product,
                    cancellationToken);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Delete product - Admin only
        [Authorize(Policy = "CanManageProducts")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            var deleted =
                await _productService.DeleteAsync(
                    id,
                    cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("This is a test exception.");
        }

        // Upload product image - Admin only
        [Authorize(Policy = "CanManageProducts")]
        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(
            int id,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var allowedExtensions =
                new[] { ".jpg", ".jpeg", ".png", ".webp" };

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(
                    "Only JPG, PNG, and WEBP images are allowed.");
            }

            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                return BadRequest(
                    "File size cannot exceed 5 MB.");
            }

            var product =
                await _productService.GetByIdAsync(
                    id,
                    cancellationToken);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var folder =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "product-images");

            Directory.CreateDirectory(folder);

            var fileName =
                $"{Guid.NewGuid()}{extension}";

            var filePath =
                Path.Combine(folder, fileName);

            using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create);

            await file.CopyToAsync(
                stream,
                cancellationToken);

            var imagePath =
                $"product-images/{fileName}";

            await _productService.UpdateImagePathAsync(
                id,
                imagePath,
                cancellationToken);

            return Ok(new
            {
                Message = "Image uploaded successfully.",
                FileName = fileName
            });
        }

        //Stock management - Admin only
        [Authorize(Policy = "CanManageProducts")]
        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(
    int id,
    [FromBody] UpdateStockDto dto,
    CancellationToken cancellationToken)
        {
            var inventory =
                await _productService.UpdateStockAsync(
                    id,
                    dto.Quantity,
                    cancellationToken);

            if (inventory == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(inventory);
        }



        //Stock Bulk Management - Admin only
        [Authorize(Policy = "CanManageProducts")]
        [HttpPut("bulk-stock")]
        public async Task<IActionResult> BulkUpdateStock(
    [FromBody] List<BulkInventoryUpdateDto> items,
    CancellationToken cancellationToken)
        {
            if (items == null || items.Count == 0)
            {
                return BadRequest(
                    "At least one inventory item is required.");
            }

            var inventoryList =
                await _productService.BulkUpdateStockAsync(
                    items,
                    cancellationToken);

            return Ok(inventoryList);
        }



        // Download product image - public
        [HttpGet("{id}/image")]
        public async Task<IActionResult> DownloadImage(
            int id,
            CancellationToken cancellationToken)
        {
            var product =
                await _productService.GetByIdAsync(
                    id,
                    cancellationToken);

            if (product == null ||
                string.IsNullOrEmpty(product.ImagePath))
            {
                return NotFound(
                    "Product image not found.");
            }

            var filePath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    product.ImagePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(
                    "Image file not found.");
            }

            var contentType =
                Path.GetExtension(filePath)
                    .ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

            var fileBytes =
                await System.IO.File.ReadAllBytesAsync(
                    filePath,
                    cancellationToken);

            return File(
                fileBytes,
                contentType);
        }
    }
}

