namespace VNUK.Dtos.ExamScheduleDto
{
    public class TeacherScheduleDto
    {
        public string SubjectID { get; set; }

        public string SemesterName { get; set; }

        public string Subjectname { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public DateTime ExamDate { get; set; }

        public string RoomName { get; set; }

        public string ExamTypeName { get; set; }

        public string Duration { get; set; }
    }
}
