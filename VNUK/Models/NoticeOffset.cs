using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("NoticeOffset", Schema = "dbo")]
    public class NoticeOffset
    {
        [Key]
        public int NoticeOffsetID { get; set; }

        public string CourseClassID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public string ClassPeriodID { get; set; }

        public string RoomID { get; set; }

        public int WeekOffset {  get; set; }

        public string Note {  get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateOffset { get; set; }
    }
}
