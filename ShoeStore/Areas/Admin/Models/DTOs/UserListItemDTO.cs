using System;
using System.Collections.Generic;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class UserListItemDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
        public int TotalOrders { get; set; }
    }
}
