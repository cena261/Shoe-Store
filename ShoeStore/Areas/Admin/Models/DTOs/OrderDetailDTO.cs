using System;
using System.Collections.Generic;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class OrderDetailDTO
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingName { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingTenDuong { get; set; }
        public string ShippingXaQuan { get; set; }
        public string ShippingTinhThanh { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}
