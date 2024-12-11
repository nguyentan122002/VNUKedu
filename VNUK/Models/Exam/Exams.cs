using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models.Exam
{
    [Table("Exams", Schema = "dbo")]
    public class Exams
    {
        [Key]
        public string ExamID { get; set; }

        public string SemesterID { get; set; }

        public DateTime ExamDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int? ExamTypeID { get; set; }

        public string SubjectsID { get; set; }

        public string? Duration { get; set; }

        public int? CourseClass_ID { get; set; }

        [ForeignKey(nameof(SemesterID))]
        public virtual Semesters Semesters { get; set; }

        [ForeignKey(nameof(ExamTypeID))]
        public virtual ExamTypes ExamTypes { get; set; }

        [ForeignKey(nameof(SubjectsID))]
        public virtual Subjects Subjects { get; set; }

        [ForeignKey(nameof(CourseClass_ID))]
        public virtual CourseClasses CourseClasses { get; set; }
    }
}
