using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressID { get; set; }

        public int UserID { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [Required, MaxLength(100)]
        public string TenDuong { get; set; }

        [Required, MaxLength(100)]
        public string XaQuan { get; set; }

        [Required, MaxLength(100)]
        public string TinhThanh { get; set; }

        public bool IsDefault { get; set; }

        [ForeignKey("UserID")]
        public virtual Users Users { get; set; }
    }
}
