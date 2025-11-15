namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class OrderResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
