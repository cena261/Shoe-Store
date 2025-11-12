using System.Collections.Generic;

namespace ShoeStore.Models.DTOs
{
    public class ProductDetailDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal DisplayPrice => PromotionPrice ?? Price;
        public decimal? Rating { get; set; }

        public string SelectedStyleColor { get; set; }
        public string SelectedColorName { get; set; }

        public List<ProductColorVariantDTO> AvailableColors { get; set; }

        public List<ProductImageDTO> Images { get; set; }

        public List<SizeVariantDTO> AvailableSizes { get; set; }
    }

    public class ProductColorVariantDTO
    {
        public string StyleColor { get; set; }
        public string ColorName { get; set; }
        public string ThumbnailImage { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class ProductImageDTO
    {
        public string ImageURL { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class SizeVariantDTO
    {
        public int VariantID { get; set; }
        public string SizeValue { get; set; }
        public int StockQty { get; set; }
        public bool IsAvailable { get; set; }
        public decimal? PriceOverride { get; set; }
    }

    public class ProductDetailResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ProductDetailDTO Product { get; set; }
    }
}
