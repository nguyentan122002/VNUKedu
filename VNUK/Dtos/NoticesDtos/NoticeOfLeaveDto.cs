namespace VNUK.Dtos.Notices
{
    public class NoticeOfLeaveDto
    {
        public int AdjustmentID { get; set; }

        public string CourseClassID { get; set; }

        public string? SubjectName { get; set; }

        public string? Reason { get; set; }

        //public DateTime CreatedDate { get; set; } 

        public string CreatedBy { get; set; }

        public int WeekOff { get; set; }

        public string ClassPeriodName { get; set; }

        public string DayofWeekName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
