using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Permissions", Schema = "dbo")]
    public class Permission
    {
        [Key]
        public int PermissionID { get; set; }

        public string PermissionName { get; set; }
    }
}
