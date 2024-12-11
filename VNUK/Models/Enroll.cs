using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Enroll", Schema = "dbo")]
    public class Enroll
    {
        [Key]
        public int EnrollID { get; set; }

        public string StudentID { get; set; }

        public int CourseClass_ID { get; set; }

        [ForeignKey(nameof(StudentID))]
        public virtual Students Students { get; set; }

        [ForeignKey(nameof(CourseClass_ID))]
        public virtual CourseClasses CourseClasses { get; set; }
    }
}
