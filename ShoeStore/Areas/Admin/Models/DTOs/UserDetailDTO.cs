using System;
using System.Collections.Generic;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class UserDetailDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
        public List<UserOrderDTO> Orders { get; set; }
        public List<UserAddressDTO> Addresses { get; set; }
    }

    public class UserOrderDTO
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public int TotalItems { get; set; }
    }

    public class UserAddressDTO
    {
        public int AddressID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string TenDuong { get; set; }
        public string XaQuan { get; set; }
        public string TinhThanh { get; set; }
        public bool IsDefault { get; set; }
    }
}
