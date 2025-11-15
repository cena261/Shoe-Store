using System;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class OrderListItemDTO
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingTinhThanh { get; set; }
        public string UserEmail { get; set; }
    }
}
