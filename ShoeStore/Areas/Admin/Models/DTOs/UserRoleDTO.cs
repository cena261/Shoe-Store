using System.Collections.Generic;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class UserRoleDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> CurrentRoles { get; set; }
        public List<RoleOptionDTO> AvailableRoles { get; set; }
    }

    public class RoleOptionDTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
