using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models.Exam
{
    [Table("ExamTypes", Schema = "dbo")]
    public class ExamTypes
    {
        [Key]
        public int ExamTypeID { get; set; }

        public string ExamTypeName { get; set; }
    }
}
