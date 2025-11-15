namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class OrderStatusUpdateDTO
    {
        public int OrderID { get; set; }
        public string NewStatus { get; set; }
    }
}
