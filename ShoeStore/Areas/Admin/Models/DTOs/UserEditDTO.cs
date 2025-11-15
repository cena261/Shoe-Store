using System.ComponentModel.DataAnnotations;

namespace ShoeStore.Areas.Admin.Models.DTOs
{
    public class UserEditDTO
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Phone must be 10-11 digits starting with 0")]
        public string Phone { get; set; }

        public bool IsActive { get; set; }
    }
}
