using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.ExamScheduleDto;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamScheduleController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public ExamScheduleController(VNUK_DbContext context) 
        {
            _context = context;
        }

        [HttpGet("GetStudentExamSchedule")]
        [Authorize("Student")]
        public async Task<ActionResult<List<StudentsExamScheduleDto>>> GetStudentExamSchedule(string SemestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var studentId = await _context.Students
               .Where(s => s.UserID == userId)
               .Select(s => s.StudentID)
               .SingleOrDefaultAsync();

            var schedule = await (from exam in _context.Exams
                                  join semester in _context.Semesters on exam.SemesterID equals semester.SemestersID
                                  join subject in _context.Subjects on exam.SubjectsID equals subject.SubjectsID
                                  join roomAlocation in _context.ExamRoomAllocations on exam.ExamID equals roomAlocation.ExamID
                                  join room in _context.Rooms on roomAlocation.RoomID equals room.RoomID
                                  join examType in _context.ExamTypes on exam.ExamTypeID equals examType.ExamTypeID
                                  join studentExam in _context.StudentExams on exam.ExamID equals studentExam.ExamID
                                  join student in _context.Students on studentExam.StudentID equals student.StudentID
                                  where student.UserID == userId && semester.SemestersID == SemestersID
                                  select new StudentsExamScheduleDto
                                  {
                                      SemesterName = semester.SemestersName,
                                      SubjectID = subject.SubjectsID,
                                      Subjectname = subject.SubjectsName,
                                      StartTime = exam.StartTime,
                                      ExamDate = exam.ExamDate,
                                      RoomName = room.RoomName,
                                      ExamTypeName = examType.ExamTypeName,
                                      Duration = exam.Duration
                                  }).Distinct().ToListAsync();

            if (schedule == null)
            {
                return NotFound("Không tìm thấy lịch thi");
            }
            return Ok(schedule);

        }

        [HttpGet("GetTeacherSchedule")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<TeacherScheduleDto>>> GetTeacherSchedule(string SemestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var studentId = await _context.Teachers
               .Where(s => s.UserID == userId)
               .Select(s => s.TeacherID)
               .SingleOrDefaultAsync();

            var schedule = await (from exam in _context.Exams
                                  join semester in _context.Semesters on exam.SemesterID equals semester.SemestersID
                                  join subject in _context.Subjects on exam.SubjectsID equals subject.SubjectsID
                                  join roomAlocation in _context.ExamRoomAllocations on exam.ExamID equals roomAlocation.ExamID
                                  join room in _context.Rooms on roomAlocation.RoomID equals room.RoomID
                                  join examType in _context.ExamTypes on exam.ExamTypeID equals examType.ExamTypeID
                                  join proctor in _context.Proctors on exam.ExamID equals proctor.ExamID
                                  join teacher in _context.Teachers on proctor.TeacherID equals teacher.TeacherID
                                  where teacher.UserID == userId && semester.SemestersID == SemestersID
                                  select new StudentsExamScheduleDto
                                  {
                                      SemesterName = semester.SemestersName,
                                      SubjectID = subject.SubjectsID,
                                      Subjectname = subject.SubjectsName,
                                      StartTime = exam.StartTime,
                                      EndTime = exam.EndTime,
                                      ExamDate = exam.ExamDate,
                                      RoomName = room.RoomName,
                                      ExamTypeName = examType.ExamTypeName,
                                      Duration = exam.Duration
                                  }).Distinct().ToListAsync();

            if (schedule == null)
            {
                return NotFound("Không tìm thấy lịch");
            }
            return Ok(schedule);
        }
    }
}
