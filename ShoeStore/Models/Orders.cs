using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("Orders")]
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        public int UserID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string OrderStatus { get; set; }

        [Required, MaxLength(100)]
        public string ShippingName { get; set; }

        [Required, MaxLength(20)]
        public string ShippingPhone { get; set; }

        [Required, MaxLength(255)]
        public string ShippingTenDuong { get; set; }

        [Required, MaxLength(100)]
        public string ShippingXaQuan { get; set; }

        [Required, MaxLength(100)]
        public string ShippingTinhThanh { get; set; }

        [ForeignKey("UserID")]
        public virtual Users Users { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}
