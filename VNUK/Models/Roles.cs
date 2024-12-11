using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Roles", Schema = "dbo")]
    public class Roles
    {
        [Key]
        public int RoleID { get; set; }

        public string RoleName { get; set; }

    }
}
