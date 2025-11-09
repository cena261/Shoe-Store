using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        [Required, MaxLength(50)]
        [Index(IsUnique = true)]
        public string CategoryName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(500)]
        public string ImageURL { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
