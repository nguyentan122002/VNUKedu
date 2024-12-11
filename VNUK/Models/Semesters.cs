using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("Semesters", Schema = "dbo")]
    public class Semesters
    {
        [Key]
        public string SemestersID { get; set; }

        public string SemestersName { get; set;}

        public int WeekStart { get; set; }

        public int WeekEnd { get; set; }

    }
}
