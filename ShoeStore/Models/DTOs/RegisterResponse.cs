using System;

namespace ShoeStore.Models.DTOs
{
    public class RegisterResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public UserData User { get; set; }
    }

    public class UserData
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] Roles { get; set; }
    }
}
