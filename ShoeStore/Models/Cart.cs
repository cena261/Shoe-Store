using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }

        public int UserID { get; set; }
        public int VariantID { get; set; }
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; }

        [ForeignKey("UserID")]
        public virtual Users Users { get; set; }

        [ForeignKey("VariantID")]
        public virtual ProductVariants ProductVariant { get; set; }
    }
}
