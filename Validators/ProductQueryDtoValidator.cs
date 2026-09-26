using EcommerceAPI.DTOs;
using FluentValidation;

namespace EcommerceAPI.Validators
{
    public class ProductQueryDtoValidator : AbstractValidator<ProductQueryDto>
    {
        public ProductQueryDtoValidator()
        {
            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage("Search cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Search));

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .When(x => x.CategoryId.HasValue)
                .WithMessage("CategoryId must be greater than 0.");

            RuleFor(x => x.BrandId)
                .GreaterThan(0)
                .When(x => x.BrandId.HasValue)
                .WithMessage("BrandId must be greater than 0.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("Minimum price cannot be negative.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Maximum price cannot be negative.");

            RuleFor(x => x)
                .Must(x =>
                    !x.MinPrice.HasValue ||
                    !x.MaxPrice.HasValue ||
                    x.MinPrice <= x.MaxPrice)
                .WithMessage("Minimum price cannot be greater than maximum price.");

            RuleFor(x => x.MinRating)
                .InclusiveBetween(0, 5)
                .When(x => x.MinRating.HasValue)
                .WithMessage("Minimum rating must be between 0 and 5.");

            RuleFor(x => x.SortBy)
                .Must(x =>
                    string.IsNullOrWhiteSpace(x) ||
                    x.Equals("name", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("price", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("rating", StringComparison.OrdinalIgnoreCase))
                .WithMessage("SortBy must be name, price, or rating.");

            RuleFor(x => x.SortOrder)
                .Must(x =>
                    string.IsNullOrWhiteSpace(x) ||
                    x.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("desc", StringComparison.OrdinalIgnoreCase))
                .WithMessage("SortOrder must be asc or desc.");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}