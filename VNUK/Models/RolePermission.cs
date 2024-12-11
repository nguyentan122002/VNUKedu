using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace VNUK.Models
{
    [Table("RolePermissions", Schema = "dbo")]
    public class RolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }

        public int PermissionID { get; set; }

        public int RoleID { get; set; }

        [ForeignKey(nameof(PermissionID))]
        public virtual Permission Permission { get; set; }

        [ForeignKey(nameof(RoleID))]
        public virtual Roles Roles { get; set; }
    }
}
