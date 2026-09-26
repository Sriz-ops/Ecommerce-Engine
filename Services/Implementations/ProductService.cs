using AutoMapper;
using EcommerceAPI.DTOs;
using EcommerceAPI.DTOs.Inventory;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Inventory;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ProductService(
            IProductRepository productRepository,
            IMapper mapper,
            IMemoryCache cache)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<ProductDto>> GetAllAsync(
    int customerId,
    CancellationToken cancellationToken)
        {
            const string cacheKey = "products:all";

            if (_cache.TryGetValue(
                cacheKey,
                out List<ProductDto>? cachedProducts))
            {
                return cachedProducts!;
            }

            var products = await _productRepository.GetAllAsync(
                cancellationToken);

            var productDtos =
                _mapper.Map<List<ProductDto>>(products);

            _cache.Set(
                cacheKey,
                productDtos,
                TimeSpan.FromHours(2));

            return productDtos;
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);

            if (product == null)
            {
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductPagedResponseDto> SearchAsync(
            ProductQueryDto query, CancellationToken cancellationToken)
        {
            var result = await _productRepository.SearchAsync(
                query.Search,
                query.CategoryId,
                query.BrandId,
                query.MinPrice,
                query.MaxPrice,
                query.MinRating,
                query.SortBy,
                query.SortOrder,
                query.Page,
                query.PageSize,
                cancellationToken);
            return new ProductPagedResponseDto
            {
                Products = _mapper.Map<List<ProductDto>>(result.Products),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = result.TotalPages
            };
        }


        public async Task<int> CreateAsync(Product product, CancellationToken cancellationToken)
        {
          var productId = await _productRepository.CreateAsync(product, cancellationToken);
            _cache.Remove("products:all");
            return productId;
        }

        public async Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken)
        {
            var updated = await _productRepository.UpdateAsync(id, product, cancellationToken);

            if(updated)
            {
                _cache.Remove("products:all");
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var deleted =
                await _productRepository.DeleteAsync(
                    id,
                    cancellationToken);

            if (deleted)
            {
                _cache.Remove("products:all");
            }

            return deleted;
        }

        public async Task<Inventory?> UpdateStockAsync(int productId,int quantity,
        CancellationToken cancellationToken)
        {
            var inventory =
                await _productRepository.UpdateStockAsync(
                    productId,
                    quantity,
                    cancellationToken);

            if (inventory != null)
            {
                //if prod list contains stock info, removing old cache
                _cache.Remove("products:all");
            }

            return inventory;
        }

        //for updating a stock in bulk
        public async Task<List<Inventory>> BulkUpdateStockAsync(
    List<BulkInventoryUpdateDto> items,
    CancellationToken cancellationToken)
        {
            var inventoryList =
                await _productRepository.BulkUpdateStockAsync(
                    items,
                    cancellationToken);

            if (inventoryList.Count > 0)
            {
                _cache.Remove("products:all");
            }

            return inventoryList;
        }

        public async Task<bool> UpdateImagePathAsync(
      int id,
      string imagePath,
      CancellationToken cancellationToken)
        {
            var updated =
                await _productRepository.UpdateImagePathAsync(
                    id,
                    imagePath,
                    cancellationToken);

            if (updated)
            {
                _cache.Remove("products:all");
            }

            return updated;
        }
    }
}