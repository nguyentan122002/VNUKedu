namespace VNUK.Dtos.Notices
{
    public class NoticeOffsetDto
    {
        public string NoticeOffsetID { get; set; }

        public string CourseClassID { get; set; }

        public string SubjectID { get; set; }

        public string SubjectName { get; set; }

        public string CreatedBy { get; set; }

        //public DateTime CreatedDate { get; set; }

        public string ClassPeriodname { get; set; }

        public string DayofWeekName { get; set; }

        public string RoomName { get; set; }

        public int WeekOffset { get; set; }

        public string Note { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateOffset { get; set; }
    }
}
