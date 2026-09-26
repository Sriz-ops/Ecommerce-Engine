using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using EcommerceAPI.DTOs;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IMemoryCache cache)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            const string cacheKey = "categories:all";

            if (_cache.TryGetValue(cacheKey, out List<CategoryDto>? cachedCategories))
            {
                return cachedCategories!;
            }

            var categories = await _categoryRepository.GetAllAsync();

            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

            _cache.Set(
                cacheKey,
                categoryDtos,
                TimeSpan.FromHours(2));

            return categoryDtos;
        }
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category =
                await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return null;
            }

            return _mapper.Map<CategoryDto>(category);
        }
    }
}