using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("ClassPeriod", Schema = "dbo")]
    public class ClassPeriod
    {
        [Key]
        public string ClassPeriodID { get; set; }

        public string ClassPeriodName { get; set; }

        public string DayofWeeksID { get; set; }

        [ForeignKey(nameof(DayofWeeksID))]
        public virtual DayofWeeks DayofWeeks { get; set; }
    }
}
