using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Models.DTOs
{
    public class AddressRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Phone must be 10 digits and start with 0")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Street name is required")]
        [MaxLength(100)]
        public string TenDuong { get; set; }

        [Required(ErrorMessage = "Ward/District is required")]
        [MaxLength(100)]
        public string XaQuan { get; set; }

        [Required(ErrorMessage = "Province/City is required")]
        [MaxLength(100)]
        public string TinhThanh { get; set; }

        public bool IsDefault { get; set; }
    }
}
