using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace VNUK.Models
{
    [Table("Users", Schema = "dbo")]
    public class Users 
    {
        [Key]
        public int UserID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int RoleID { get; set; }

        [ForeignKey(nameof(RoleID))]
        public virtual Roles Role { get; set; }

    }
}
