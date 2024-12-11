using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Subjects", Schema = "dbo")]
    public class Subjects
    {
        [Key]
        public string SubjectsID { get; set; }

        public string SubjectsName { get; set; }

        public int NumberOfCredits { get; set; }
         
        public int WeekStart { get; set; }

        public int WeekEnd { get; set; }    

        public string MajorID { get; set; }

        [ForeignKey(nameof(SubjectsID))]
        public virtual Majors Majors { get; set; }
    }
}
