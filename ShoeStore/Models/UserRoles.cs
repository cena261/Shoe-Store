using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStore.Models
{
    [Table("UserRoles")]
    public class UserRoles
    {
        [Key, Column(Order = 0)]
        public int UserID { get; set; }

        [Key, Column(Order = 1)]
        public int RoleID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public DateTime AssignedAt { get; set; }

        [ForeignKey("UserID")]
        public virtual Users User { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}
