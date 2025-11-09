using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("Reviews")]
    public class Reviews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }

        public int ProductID { get; set; }
        public int UserID { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ReviewDate { get; set; }

        [ForeignKey("ProductID")]
        public virtual Products Product { get; set; }

        [ForeignKey("UserID")]
        public virtual Users Users { get; set; }
    }
}
