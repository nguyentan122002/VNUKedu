using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VNUK.Models
{
    [Table("ClassNotifications", Schema = "dbo")]
    public class ClassNotifications
    {
        [Key]
        public int NotificationID { get; set; }

        public string CourseClassID { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsImportant { get; set; }

        public bool IsDeleted { get; set; }
    }
}
