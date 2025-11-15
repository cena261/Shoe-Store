using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class ProductVariantDTO
    {
        public int? VariantID { get; set; }

        [Required(ErrorMessage = "Size is required")]
        [MaxLength(10)]
        public string SizeValue { get; set; }

        [Required(ErrorMessage = "Color name is required")]
        [MaxLength(250)]
        public string ColorName { get; set; }

        [MaxLength(50)]
        public string StyleColor { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [MaxLength(250)]
        public string SKU { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater")]
        public int StockQty { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or greater")]
        public decimal? PriceOverride { get; set; }

        public bool IsActive { get; set; }
    }
}
