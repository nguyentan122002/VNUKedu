using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models.Exam
{
    [Table("Proctors", Schema = "dbo")]
    public class Proctors
    {
        [Key]
        public string ProctorID { get; set; }

        public string TeacherID { get; set; }

        public string ProctorRole { get; set; }

        public string? ExamID { get; set; }

        [ForeignKey(nameof(TeacherID))]
        public virtual Teachers Teachers { get; set; }

        [ForeignKey(nameof(ExamID))]
        public virtual Exams Exams { get; set; }
    }
}
