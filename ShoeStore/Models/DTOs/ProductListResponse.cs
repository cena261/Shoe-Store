using System.Collections.Generic;

namespace ShoeStore.Models.DTOs
{
    public class ProductListResponse
    {
        public bool Status { get; set; }
        public List<ProductListItemDTO> Products { get; set; }
        public int TotalProducts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
