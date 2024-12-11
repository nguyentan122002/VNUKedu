using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("CourseClass", Schema = "dbo")]
    public class CourseClasses
    {
        [Key]
        public int Id { get; set; }

        public string CourseClassID { get; set; } 

        public string CourseClassName { get; set; }

        public string TeacherID { get; set; }

        public string SubjectsID { get; set; }

        public string RoomID { get; set; }

        public string? DepartmentID { get; set; }

        public string SemestersID { get; set; }

        public string ClassPeriodID { get; set; }

        public string? Note {  get; set; }

        [ForeignKey(nameof(TeacherID))]
        public virtual Teachers Teacher { get; set; }

        [ForeignKey(nameof(SubjectsID))]
        public virtual Subjects Subject { get; set; }

        [ForeignKey(nameof(RoomID))]
        public virtual Rooms Room { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public virtual Departments Department { get; set; }

        [ForeignKey(nameof(SemestersID))]
        public virtual Semesters Semesters { get; set; }

        [ForeignKey(nameof(ClassPeriodID))]
        public virtual ClassPeriod ClassPeriod { get; set; }
    }
}
