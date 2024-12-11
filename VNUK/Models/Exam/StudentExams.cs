using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models.Exam
{
    [Table("StudentExams", Schema = "dbo")]
    public class StudentExams
    {
        [Key]
        public int StudentExamID { get; set; }

        public string ExamID { get; set; }

        public string StudentID { get; set; }

        public decimal? Score { get; set; }

        [ForeignKey(nameof(ExamID))]
        public virtual Exams Exams { get; set; }

        [ForeignKey(nameof(StudentID))]
        public virtual Students Students { get; set; }
    }
}
