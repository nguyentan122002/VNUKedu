using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Teachers", Schema = "dbo")]
    public class Teachers
    {
        [Key]
        public string TeacherID { get; set; }

        public string TeacherName { get; set;}

        public bool IsCVHT { get; set; }


        public int UserID { get; set; }

        public string DepartmentID { get; set; }


        [ForeignKey(nameof(DepartmentID))]
        public virtual Departments Department { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual Users User { get; set; }
    }
}
