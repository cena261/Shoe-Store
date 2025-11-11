using System.Collections.Generic;

namespace ShoeStore.Models.DTOs
{
    public class ProductListItemDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public string CategoryName { get; set; }
        public string Gender { get; set; }
        public List<ColorVariantDTO> ColorVariants { get; set; }
    }

    public class ColorVariantDTO
    {
        public string StyleColor { get; set; }
        public string ColorName { get; set; }
        public string MainImageURL { get; set; }
    }
}
