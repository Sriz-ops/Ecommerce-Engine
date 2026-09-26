using AutoMapper;
using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class ProductPriceService : IProductPriceService
    {
        private readonly IProductPriceRepository _repository;
        private readonly IMapper _mapper;

        public ProductPriceService(
            IProductPriceRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductPriceDto?> GetByIdAsync(int id)
        {
            var price = await _repository.GetByIdAsync(id);

            if (price == null)
            {
                return null;
            }

            return _mapper.Map<ProductPriceDto>(price);
        }

        public async Task<ProductPriceDto?> GetCurrentPriceByProductIdAsync(
            int productId)
        {
            var price =
                await _repository.GetCurrentPriceByProductIdAsync(productId);

            if (price == null)
            {
                return null;
            }

            return _mapper.Map<ProductPriceDto>(price);
        }

        public async Task<int> AddAsync(CreateProductPriceDto dto)
        {
            var productPrice = _mapper.Map<ProductPrice>(dto);

            return await _repository.AddAsync(productPrice);
        }

    }
}