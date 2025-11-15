using System;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class ProductListItemDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public string CategoryName { get; set; }
        public int TotalVariants { get; set; }
        public int TotalStock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FirstImageURL { get; set; }
    }
}
