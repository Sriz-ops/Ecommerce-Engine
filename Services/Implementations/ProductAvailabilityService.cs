using System.Threading;
using AutoMapper;
using EcommerceAPI.DTO;
using EcommerceAPI.DTOs;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class ProductAvailabilityService
        : IProductAvailabilityService
    {
        private readonly IProductAvailabilityRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductAvailabilityService> _logger;

        public ProductAvailabilityService(
            IProductAvailabilityRepository repository,
            IMapper mapper,
            ILogger<ProductAvailabilityService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductAvailabilityDto?> GetAvailabilityAsync(
            int productId, CancellationToken cancellationToken)
        {
            var result =
                await _repository.GetAvailabilityAsync(productId, cancellationToken);

            if (result == null)
            
            {
                _logger.LogWarning(
           "Product {ProductId} was not found when checking availability",
           productId);

                return null;
            }
            _logger.LogInformation(
       "Checked availability for Product {ProductId}: Available={IsAvailable}, Stock={StockQuantity}",
       productId,
       result.IsAvailable,
       result.StockQuantity);

            return _mapper.Map<ProductAvailabilityDto>(result);
        }



        public async Task<List<InventoryCheckResultDto>>
    CheckInventoryAsync(
        List<InventoryCheckItemDto> items, CancellationToken cancellationToken)
        {
            var models = _mapper.Map<
                List<Models.InventoryCheckItem>>(items);

            var results =
                await _repository.CheckInventoryAsync(models, cancellationToken);

            _logger.LogInformation(
                "Inventory checked for {ProductCount} products",
                items.Count);

            return _mapper.Map<
                List<InventoryCheckResultDto>>(results);
        }
    }
}