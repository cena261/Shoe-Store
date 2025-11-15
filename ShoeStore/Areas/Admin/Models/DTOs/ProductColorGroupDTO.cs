using System.Collections.Generic;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class ProductColorGroupDTO
    {
        public string ColorName { get; set; }
        public string StyleColor { get; set; }
        public string ImageURL { get; set; }
        public List<ProductVariantDTO> Variants { get; set; }

        public ProductColorGroupDTO()
        {
            Variants = new List<ProductVariantDTO>();
        }
    }
}
