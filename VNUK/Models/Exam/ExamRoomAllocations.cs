using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models.Exam
{
    [Table("ExamRoomAllocations", Schema = "dbo")]
    public class ExamRoomAllocations
    {
        [Key]
        public int AllocationID { get; set; }

        public string ExamID { get; set; }

        public string RoomID { get; set; }

        public int StudentExamID { get; set; }

        public int NumberOfSeatAllocated { get; set; }

        [ForeignKey(nameof(ExamID))]
        public virtual Exams Exams { get; set; }

        [ForeignKey(nameof(RoomID))]
        public virtual Rooms Room { get; set; }

        [ForeignKey(nameof(StudentExamID))]
        public virtual StudentExams StudentExams { get; set; }
    }
}
