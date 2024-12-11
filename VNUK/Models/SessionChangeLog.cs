using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("SessionChangeLog", Schema = "dbo")]
    public class SessionChangeLog
    {
        [Key]
        public int SessionChangeID { get; set; }

        public int CourseClass_ID { get; set; }

        public string OldRoomID {  get; set; }

        public string NewRoomID { get; set; }

        public string OldClassPeriodID { get; set; }

        public string NewClassPeriodID { get; set; }

        public int OldWeek {  get; set; }

        public int NewWeek { get; set; }

        public DateTime ChangeDate { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public string Reason { get; set; }

        [ForeignKey(nameof(CourseClass_ID))]
        public virtual CourseClasses CourseClasses { get; set; }

        [ForeignKey(nameof(OldClassPeriodID))]
        public virtual ClassPeriod OldClassPeriod { get; set; }

        [ForeignKey(nameof(NewClassPeriodID))]
        public virtual ClassPeriod NewClassPeriod { get;set; }

        [ForeignKey(nameof(OldRoomID))]
        public virtual Rooms OldRoom { get; set; }

        [ForeignKey(nameof(NewRoomID))]
        public virtual Rooms NewRoom { get; set; }

    }
}
