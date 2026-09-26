using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using EcommerceAPI.DTOs;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        public BrandService(
            IBrandRepository brandRepository,
            IMapper mapper,
            IMemoryCache cache)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<BrandDto>> GetAllAsync()
        {
            const string cacheKey = "brands:all";

            if (_cache.TryGetValue(cacheKey, out List<BrandDto>? cachedBrands))
            {
                return cachedBrands!;
            }

            var brands = await _brandRepository.GetAllAsync();

            var brandDtos = _mapper.Map<List<BrandDto>>(brands);

            _cache.Set(
                cacheKey,
                brandDtos,
                TimeSpan.FromHours(2));

            return brandDtos;
        }

        public async Task<BrandDto?> GetByIdAsync(int id)
        {
            var brand =
                await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                return null;
            }

            return _mapper.Map<BrandDto>(brand);
        }
    }
}