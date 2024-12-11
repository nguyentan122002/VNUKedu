using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("DayofWeeks", Schema = "dbo")]
    public class DayofWeeks
    {
        [Key]
        public string DayofWeeksID { get; set; }

        public string DayofWeeksName { get; set; }
    }
}
