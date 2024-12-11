using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Students", Schema = "dbo")]
    public class Students
    {
        [Key]
        public string StudentID { get; set; }

        public string StudentName { get; set; }

        public DateTime StudentDateOfBirth { get; set; }

        public string Email { get; set; }

        public decimal ScoreEnglish { get; set; }

        public string MajorID { get; set; }

        public int UserID { get; set; }

        public string ClassID { get; set; }

        [ForeignKey(nameof(MajorID))]
        public virtual Majors Majors { get; set; }

        [ForeignKey(nameof(UserID))]
        public virtual Users Users { get; set; }

        [ForeignKey(nameof(ClassID))]
        public virtual Classes Class { get; set; }

    }
}
