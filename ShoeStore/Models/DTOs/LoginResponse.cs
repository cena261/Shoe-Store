using System;

namespace ShoeStore.Models.DTOs
{
    public class LoginResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public UserData User { get; set; }
    }
}
