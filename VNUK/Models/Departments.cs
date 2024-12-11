using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Departments", Schema = "dbo")]
    public class Departments
    {
        [Key]
        public string DepartmentID { get; set; }

        public string DepartmentName { get; set; }

    }
}
