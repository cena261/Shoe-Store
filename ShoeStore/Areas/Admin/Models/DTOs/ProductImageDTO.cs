using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class ProductImageDTO
    {
        public int? ImageID { get; set; }

        [MaxLength(50)]
        public string StyleColor { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [MaxLength(500)]
        public string ImageURL { get; set; }

        public int? DisplayOrder { get; set; }
    }
}
