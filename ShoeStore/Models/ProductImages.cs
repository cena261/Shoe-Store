using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("ProductImages")]
    public class ProductImages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }

        public int ProductID { get; set; }

        [MaxLength(50)]
        public string StyleColor { get; set; }

        [Required, MaxLength(500)]
        [Index(IsUnique = true)]
        public string ImageURL { get; set; }

        public int? DisplayOrder { get; set; }

        [ForeignKey("ProductID")]
        public virtual Products Product { get; set; }
    }
}
