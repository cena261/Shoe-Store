namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class ProductResponseDTO
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int? ProductID { get; set; }
        public object Data { get; set; }
    }
}
