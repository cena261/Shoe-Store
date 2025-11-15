namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class UserResponseDTO
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
