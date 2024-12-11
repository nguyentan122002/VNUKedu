using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("NoticeOfLeave", Schema = "dbo")]
    public class NoticeOfLeave
    {
        [Key]
        public int AdjustmentID { get; set; }

        public string CourseClassID { get; set; }

        public string? Reason { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } 

        public int WeekOff { get; set; }

        public bool IsDeleted { get; set; }
    }
}
