namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class OrderItemDTO
    {
        public int OrderItemID { get; set; }
        public int VariantID { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string ImageUrl { get; set; }
    }
}
