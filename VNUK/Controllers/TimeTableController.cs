using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.TimeTableDto;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeTableController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public TimeTableController(VNUK_DbContext context) 
        {
            _context = context;
        }

        [HttpGet("getStudentTimeTable")]
        [Authorize("Student")]
        public async Task<ActionResult<List<StudentTimeTableDto>>> GetStudentTimeTable(string SemestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var timetable = await (from enroll in _context.Enrolls
                                   join student in _context.Students on enroll.StudentID equals student.StudentID
                                   join courseClass in _context.CourseClass on enroll.CourseClass_ID equals courseClass.Id
                                   join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                   join classPeriod in _context.ClassPeriods on courseClass.ClassPeriodID equals classPeriod.ClassPeriodID
                                   join dayOfWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayOfWeek.DayofWeeksID
                                   join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                   join room in _context.Rooms on courseClass.RoomID equals room.RoomID
                                   join semester in _context.Semesters on courseClass.SemestersID equals semester.SemestersID
                                   join classObj in _context.Class on student.ClassID equals classObj.ClassID
                                   join major in _context.Majors on student.MajorID equals major.MajorID
                                   where student.UserID == userId && semester.SemestersID == SemestersID
                                   select new StudentTimeTableDto
                                   {
                                       CourseClassID = courseClass.CourseClassID,
                                       CourseClassName = courseClass.CourseClassName,
                                       SubjectID = subject.SubjectsID,
                                       SubjectsName = subject.SubjectsName,
                                       NumberOfCredits = subject.NumberOfCredits,
                                       WeekStart = subject.WeekStart,
                                       WeekEnd = subject.WeekEnd,
                                       SemestersName = semester.SemestersName,
                                       TeacherName = teacher.TeacherName,
                                       RoomName = room.RoomName,
                                       ClassPeriodName = classPeriod.ClassPeriodName, 
                                       DayOfWeekName = dayOfWeek.DayofWeeksName,
                                       Note = courseClass.Note
                                   }).ToListAsync();

            if (timetable == null || !timetable.Any())
            {
                return NotFound("Không tìm thấy thời khóa biểu cho người dùng.");
            }

            var data = timetable
                .GroupBy(t => new
                {
                    t.CourseClassID,
                    t.CourseClassName,
                    t.SubjectID,
                    t.SubjectsName,
                    t.NumberOfCredits,
                    t.WeekStart,
                    t.WeekEnd,
                    t.SemestersName,
                    t.TeacherName,
                    t.RoomName,
                    t.DayOfWeekName,
                    t.Note
                })
                .Select(g => new StudentTimeTableDto
                {                    
                    CourseClassID = g.Key.CourseClassID,
                    CourseClassName = g.Key.CourseClassName,
                    SubjectID = g.Key.SubjectID,
                    SubjectsName = g.Key.SubjectsName,
                    NumberOfCredits = g.Key.NumberOfCredits,
                    WeekStart = g.Key.WeekStart,
                    WeekEnd = g.Key.WeekEnd,
                    SemestersName = g.Key.SemestersName,
                    TeacherName = g.Key.TeacherName,
                    RoomName = g.Key.RoomName,
                    DayOfWeekName = g.Key.DayOfWeekName,
                    Note = g.Key.Note,
                    ClassPeriodName = string.Join(", ", g.Select(t => t.ClassPeriodName).Distinct())
                })
                .ToList();

            return Ok(data);
        }

        [HttpGet("getTeacherTimeTable")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<TeacherTimeTableDto>>> GetTeacherTimeTable(string semestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var timetable = await (from semesters in _context.Semesters
                                join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                                join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                join room in _context.Rooms on courseClass.RoomID equals room.RoomID
                                join classPeriod in _context.ClassPeriods on courseClass.ClassPeriodID equals classPeriod.ClassPeriodID
                                join dayOfWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayOfWeek.DayofWeeksID
                                join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                join enroll in _context.Enrolls on courseClass.Id equals enroll.CourseClass_ID
                                join student in _context.Students on enroll.StudentID equals student.StudentID
                                where teacher.UserID == userId && courseClass.SemestersID == semestersID
                                select new TeacherTimeTableDto
                                {
                                    CourseClassID = courseClass.CourseClassID,
                                    CourseClassName = courseClass.CourseClassName,
                                    SubjectID = subject.SubjectsID,
                                    SubjectsName = subject.SubjectsName,
                                    NumberOfCredits = subject.NumberOfCredits,
                                    WeekStart = subject.WeekStart,
                                    WeekEnd = subject.WeekEnd,
                                    SemestersName = semesters.SemestersName,
                                    RoomName = room.RoomName,
                                    ClassPeriodName = classPeriod.ClassPeriodName,
                                    DayOfWeekName = dayOfWeek.DayofWeeksName,
                                    Note = courseClass.Note,
                                }).ToListAsync();
            if (timetable == null)
            {
                return NotFound("Không tìm thấy thời khóa biểu cho người dùng.");
            }
            var data = timetable
                .GroupBy(t => new
                {
                    t.CourseClassID,
                    t.CourseClassName,
                    t.SubjectID,
                    t.SubjectsName,
                    t.NumberOfCredits,
                    t.WeekStart,
                    t.WeekEnd,
                    t.SemestersName,
                    t.RoomName,
                    t.DayOfWeekName,
                    t.Note,
                })
                .Select(g => new TeacherTimeTableDto
                {
                    CourseClassID = g.Key.CourseClassID,
                    CourseClassName = g.Key.CourseClassName,
                    SubjectID = g.Key.SubjectID,
                    SubjectsName = g.Key.SubjectsName,
                    NumberOfCredits = g.Key.NumberOfCredits,
                    WeekStart = g.Key.WeekStart,
                    WeekEnd = g.Key.WeekEnd,
                    SemestersName = g.Key.SemestersName,
                    RoomName = g.Key.RoomName,
                    DayOfWeekName = g.Key.DayOfWeekName,
                    Note = g.Key.Note,
                    ClassPeriodName = string.Join(", ", g.Select(t => t.ClassPeriodName).Distinct())
                })
                .ToList();
            return Ok(data);
        }
    }
}
