namespace VNUK.Dtos.TimeTableDto
{
    public class TeacherTimeTableDto
    {
        public string CourseClassID { get; set; }
        public string CourseClassName { get; set; }
        public string SubjectID { get; set; }
        public string SubjectsName { get; set; }
        public int NumberOfCredits { get; set; }
        public int WeekStart { get; set; }
        public int WeekEnd { get; set; }
        public string SemestersName { get; set; }
        public string RoomName { get; set; }
        public string ClassPeriodName { get; set; }
        public string DayOfWeekName { get; set; }
        public string Note { get; set; }
    }
}
