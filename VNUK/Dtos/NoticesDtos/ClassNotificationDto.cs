using System.Diagnostics.Eventing.Reader;

namespace VNUK.Dtos.Notices
{
    public class ClassNotificationDto
    {
        public int NotificationID { get; set; }

        public string CourseClassID { get; set; }

        public string SubjectID { get; set; }

        public string SubjectName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsImportant { get; set; }

        public bool IsDeleted { get; set; } 
    }
}
