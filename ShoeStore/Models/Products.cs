using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("Products")]
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required, MaxLength(100)]
        public string ProductName { get; set; }

        [Required, MaxLength(150)]
        [Index(IsUnique = true)]
        public string Slug { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal")]
        public decimal? PromotionPrice { get; set; }

        public int CategoryID { get; set; }

        [Column(TypeName = "decimal")]
        public decimal Rating { get; set; }

        public bool IsActive { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Categories Category { get; set; }

        public virtual ICollection<ProductVariants> ProductVariants { get; set; }
        public virtual ICollection<ProductImages> ProductImages { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
    }
}
