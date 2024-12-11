using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Class", Schema = "dbo")]
    public class Classes
    {
        [Key]
        public string ClassID { get; set; }

        public string ClassName { get; set; }

        public string? DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public Departments Departments { get; set; }
    }
}
