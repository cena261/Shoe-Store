using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("PasswordResetTokens")]
    public class PasswordResetToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; }

        [Required, MaxLength(255)]
        [Index]
        public string Email { get; set; }

        [Required, MaxLength(6)]
        public string Code { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; }

        public DateTime? UsedAt { get; set; }
    }
}
