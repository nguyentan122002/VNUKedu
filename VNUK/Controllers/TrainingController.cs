using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.CommomDtos;
using VNUK.Dtos.TimeTableDto;
using VNUK.Dtos.TrainingDtos;
using VNUK.Dtos.UserDto;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public TrainingController(VNUK_DbContext context) 
        {
            _context = context;
        }

        [HttpGet("getListDepartment")]
        [Authorize(Roles = "Training")]
        public async Task<ActionResult<DepartmentDto>> GetListDepartment()
        {
            try
            {
                var data = await _context.Departments.Select(a => new DepartmentDto
                {
                    DepartmentID = a.DepartmentID,
                    DepartmentName = a.DepartmentName,
                }).ToListAsync();

                if (data == null)
                {
                    return NotFound("Không tìm thấy Khoa.");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }           
        }

        [HttpGet("getAllTeacher")]
        [Authorize(Roles = "Training")]
        public async Task<ActionResult<TeacherDto>> GetAllTeacher()
        {
            try
            {
                var data = await _context.Teachers.Select(a => new TeacherDto
                {
                    TeacherID = a.TeacherID,
                    TeacherName = a.TeacherName,
                    DepartmentName = a.Department.DepartmentName,
                    IsCVHT = a.IsCVHT
                }).ToListAsync();

                if (data == null)
                {
                    return NotFound("Không tìm thấy Giảng viên nào.");
                }
                return Ok(data);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("getTeacherofDepartment")]
        [Authorize(Roles ="Training")]
        public async Task<ActionResult<TeacherDto>> GetTeacherofDepartment(string departmentID)
        {
            try
            {
                var data = await _context.Teachers.Where(t => t.DepartmentID == departmentID).Select(a => new TeacherDto
                {
                    TeacherID = a.TeacherID,
                    TeacherName = a.TeacherName,
                    DepartmentName = a.Department.DepartmentName,
                    IsCVHT = a.IsCVHT
                }).ToListAsync();
                if (data == null)
                {
                    return NotFound("Không tìm thấy Giảng viên nào.");
                }
                return Ok(data);
            }
            catch 
            {
                return StatusCode(500);
            }
        }

        [HttpGet("getListSubjectofTeacher")]
        [Authorize(Roles = "Training")]
        public async Task<ActionResult<SubjectDto>> GetListSubjectofTeacher(string teacherID, string semesterID)
        {
            try
            {
                var subject = await (from semesters in _context.Semesters
                                     join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                                     join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                     join subjects in _context.Subjects on courseClass.SubjectsID equals subjects.SubjectsID
                                     join major in _context.Majors on subjects.MajorID equals major.MajorID
                                     where teacher.TeacherID == teacherID && semesters.SemestersID == semesterID
                                     select new SubjectDto
                                     {
                                         SubjectsID = subjects.SubjectsID,
                                         SubjectsName = subjects.SubjectsName,
                                         NumberOfCredits = subjects.NumberOfCredits,
                                         WeekStart = subjects.WeekStart,
                                         WeekEnd = subjects.WeekEnd,
                                         MajorName = major.MajorName,
                                     }).Distinct().ToListAsync();
                if (subject == null)
                {
                    return NotFound("Không tìm thấy môn học nào trong kỳ này.");
                }
                return Ok(subject);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("getTimeTableTeacher")]
        [Authorize(Roles = "Training")]
        public async Task<ActionResult<TeacherTimeTableDto>> GetTimeTableTeacher(string teacherId,string semesterId)
        {
            var data = await (from teacher in _context.Teachers
                              join courseclass in _context.CourseClass on teacher.TeacherID equals courseclass.TeacherID
                              join semester in _context.Semesters on courseclass.SemestersID equals semester.SemestersID
                              join subject in _context.Subjects on courseclass.SubjectsID equals subject.SubjectsID
                              join room in _context.Rooms on courseclass.RoomID equals room.RoomID
                              join classperiod in _context.ClassPeriods on courseclass.ClassPeriodID equals classperiod.ClassPeriodID
                              join dayofweek in _context.DayofWeeks on classperiod.DayofWeeksID equals dayofweek.DayofWeeksID
                              where teacher.TeacherID == teacherId && semester.SemestersID == semesterId
                              select new TeacherTimeTableDto
                              {
                                  CourseClassID = courseclass.CourseClassID,
                                  CourseClassName = courseclass.CourseClassName,
                                  SubjectID = subject.SubjectsID,
                                  SubjectsName = subject.SubjectsName,
                                  NumberOfCredits = subject.NumberOfCredits,
                                  WeekStart = subject.WeekStart,
                                  WeekEnd = subject.WeekEnd,
                                  SemestersName = semester.SemestersName,
                                  RoomName = room.RoomName,
                                  ClassPeriodName = classperiod.ClassPeriodName,
                                  DayOfWeekName = dayofweek.DayofWeeksName,
                                  Note = courseclass.Note,
                              }).Distinct().ToListAsync();
            if (data == null)
            {
                return NotFound("Giảng viên này không dạy môn nào.");
            }
            var timetable = data.GroupBy(t => new
            {
                t.CourseClassID,
                t.CourseClassName,
                t.SubjectID,
                t.SubjectsName,
                t.NumberOfCredits,
                t.RoomName,
                t.DayOfWeekName,
                t.SemestersName,
                t.WeekStart,
                t.WeekEnd,
                t.Note,
            }).Select(g => new TeacherTimeTableDto
            {
                CourseClassID =  g.Key.CourseClassID,
                CourseClassName = g.Key.CourseClassName,
                SubjectID = g.Key.SubjectID,
                SubjectsName = g.Key.SubjectsName,
                NumberOfCredits = g.Key.NumberOfCredits,
                WeekStart = g.Key.WeekStart,
                WeekEnd = g.Key.WeekEnd,
                SemestersName = g.Key.SemestersName,
                RoomName = g.Key.RoomName,
                ClassPeriodName = string.Join(",", g.Select(t => t.ClassPeriodName).Distinct()),
                DayOfWeekName = g.Key.DayOfWeekName,
                Note = g.Key.Note,
            }).Distinct().ToList();
            return Ok(timetable);
        }

    }
}
