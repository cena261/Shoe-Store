using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class ProductFormDTO
    {
        public int? ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Slug is required")]
        [MaxLength(150)]
        public string Slug { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Promotion price must be 0 or greater")]
        public decimal? PromotionPrice { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }

        public bool IsActive { get; set; }

        public List<ProductVariantDTO> Variants { get; set; }
        public List<ProductImageDTO> Images { get; set; }
        public List<ProductColorGroupDTO> ColorGroups { get; set; }

        public ProductFormDTO()
        {
            Variants = new List<ProductVariantDTO>();
            Images = new List<ProductImageDTO>();
            ColorGroups = new List<ProductColorGroupDTO>();
            IsActive = true;
        }
    }
}
