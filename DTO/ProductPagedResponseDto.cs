namespace EcommerceAPI.DTOs
{
    public class ProductPagedResponseDto
    {
        public List<ProductDto> Products { get; set; } = new();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
    }
}