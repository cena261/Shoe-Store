using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("OrderItems")]
    public class OrderItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }
        public int VariantID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal")]
        public decimal UnitPrice { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Subtotal { get; set; }

        [ForeignKey("OrderID")]
        public virtual Orders Order { get; set; }

        [ForeignKey("VariantID")]
        public virtual ProductVariants ProductVariant { get; set; }
    }
}
