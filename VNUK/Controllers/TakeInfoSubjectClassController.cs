using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.CommomDtos;
using VNUK.Models;
using VNUK.VNUKDbContext;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TakeInfoSubjectClassController : ControllerBase
    {
        private readonly VNUK_DbContext _context;
        public TakeInfoSubjectClassController(VNUK_DbContext context)
        {
            _context = context;
        }

        [HttpGet("GetListSubject")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<SubjectDto>>> GetListSubject(string SemestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var subject = await (from semesters in _context.Semesters
                                 join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                                 join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                 join subjects in _context.Subjects on courseClass.SubjectsID equals subjects.SubjectsID
                                 join major in _context.Majors on subjects.MajorID equals major.MajorID
                                 where teacher.UserID == userId && semesters.SemestersID == SemestersID
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
        
        [HttpGet("GetListCourseClass")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<CourseClassDto>>> GetListCourseClass(string subjectID, string semesterId)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);
            
            var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();

            var SubjectClass = await (from semesters in _context.Semesters
                                               join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                                               join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                               join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                               join department in _context.Departments on courseClass.DepartmentID equals department.DepartmentID
                                               where  teacher.TeacherID == teacherId && subject.SubjectsID == subjectID && semesters.SemestersID == semesterId
                                              select new 
                                                       {
                                                           CourseClassID = courseClass.CourseClassID,
                                                           CourseClassName = courseClass.CourseClassName,
                                                           DepartmentName = department.DepartmentName,
                                                           SemestersName = semesters.SemestersName,
                                                       }).Distinct().ToListAsync();
            var data = SubjectClass.GroupBy(t => new
                        {
                            t.CourseClassID,
                            t.CourseClassName,
                            t.DepartmentName,
                            t.SemestersName,
                        }).Select(g => new CourseClassDto
                        {
                            CourseClassName = g.Key.CourseClassName,
                            CourseClassID= g.Key.CourseClassID,
                            DepartmentName= g.Key.DepartmentName,
                            SemestersName= g.Key.SemestersName,
                        }).ToList();


            return Ok(data);
        }

    }
}
