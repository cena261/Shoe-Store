using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("ProductVariants")]
    public class ProductVariants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VariantID { get; set; }

        public int ProductID { get; set; }

        [Required, MaxLength(10)]
        public string SizeValue { get; set; }

        [Required, MaxLength(50)]
        public string ColorName { get; set; }

        [MaxLength(7)]
        public string HexCode { get; set; }

        [Required, MaxLength(64)]
        [Index(IsUnique = true)]
        public string SKU { get; set; }

        public int StockQty { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? PriceOverride { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("ProductID")]
        public virtual Products Product { get; set; }
    }
}
