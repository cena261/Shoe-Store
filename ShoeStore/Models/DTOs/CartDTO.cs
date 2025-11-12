using System.Collections.Generic;

namespace ShoeStore.Models.DTOs
{
    public class CartItemDTO
    {
        public int VariantID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Slug { get; set; }
        public string StyleColor { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int Quantity { get; set; }
        public int StockQty { get; set; }
        public string ImageURL { get; set; }
    }

    public class AddToCartRequest
    {
        public int VariantID { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int VariantID { get; set; }
        public int Quantity { get; set; }
    }

    public class CartResponseDTO
    {
        public List<CartItemDTO> Items { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
