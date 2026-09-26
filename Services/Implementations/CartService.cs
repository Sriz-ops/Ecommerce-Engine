using AutoMapper;
using EcommerceAPI.DTO;
using EcommerceAPI.DTOs;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(
            ICartRepository repository,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CartItemDto>> GetCartAsync(
            int customerId,
            CancellationToken cancellationToken)    
        {
            var items =
                await _repository.GetCartAsync(customerId, cancellationToken);

            return _mapper.Map<List<CartItemDto>>(items);
        }

        public async Task AddItemAsync(int customerId,AddCartItemDto dto, CancellationToken cancellationToken)
        {
            await _repository.AddItemAsync(
                customerId,
                dto.ProductId,
                dto.Quantity,
                cancellationToken);

            _logger.LogInformation(
            "Added Product {ProductId} with quantity {Quantity} to Customer {CustomerId} cart",
            dto.ProductId,
            dto.Quantity,
            customerId);
        }

        public async Task<bool> UpdateItemAsync(
    int customerId,
    int productId,
    UpdateCartItemDto dto, CancellationToken cancellationToken)
        {
            var updated = await _repository.UpdateItemAsync(
                customerId,
                productId,
                dto.Quantity, cancellationToken);

            if (!updated)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found in Customer {CustomerId} cart",
                    productId,
                    customerId);

                return false;
            }

            _logger.LogInformation(
                "Updated Product {ProductId} quantity to {Quantity} for Customer {CustomerId}",
                productId,
                dto.Quantity,
                customerId);

            return true;
        }

        public async Task<bool> RemoveItemAsync(
        int customerId,
        int productId, CancellationToken cancellationToken)
        {
            var removed = await _repository.RemoveItemAsync(
                customerId,
                productId,
                cancellationToken);

            if (!removed)
            {
                _logger.LogWarning(
                    "Product {ProductId} was not found in Customer {CustomerId} cart",
                    productId,
                    customerId);

                return false;
            }

            _logger.LogInformation(
                "Removed Product {ProductId} from Customer {CustomerId} cart",
                productId,
                customerId);

            return true;
        }
        public async Task ClearCartAsync(int customerId, CancellationToken cancellationToken)
        {
            await _repository.ClearCartAsync(customerId,cancellationToken);

            _logger.LogInformation(
                "Cleared cart for Customer {CustomerId}",
                customerId);
        }
        public async Task<int> GetItemCountAsync(
            int customerId, CancellationToken cancellationToken)
        {
            return await _repository.GetItemCountAsync(customerId, cancellationToken);
        }

        public async Task<decimal> GetSubtotalAsync(
            int customerId, CancellationToken cancellationToken)
        {
            return await _repository.GetSubtotalAsync(customerId, cancellationToken);
        }
    }
}