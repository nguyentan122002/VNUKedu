using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.Notices;
using VNUK.Dtos.Notices.NoticeInput;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassNotificationController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public ClassNotificationController(VNUK_DbContext context)  
        {
            _context = context;
        }

        [HttpPost("CreateClassNoticification")]
        [Authorize("Teacher")]
        public async Task<IActionResult> CreateClassNoticification(ClassNotificationInputDto classNotificationInputDto)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);
            var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();
            var SubjectClass = await (from semesters in _context.Semesters
                                      join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                                      join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                      join room in _context.Rooms on courseClass.RoomID equals room.RoomID
                                      join classPeriod in _context.ClassPeriods on courseClass.ClassPeriodID equals classPeriod.ClassPeriodID
                                      join dayOfWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayOfWeek.DayofWeeksID
                                      join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                      join department in _context.Departments on courseClass.DepartmentID equals department.DepartmentID
                                      where teacher.TeacherID == teacherId
                                      select courseClass.CourseClassID).Distinct().ToListAsync();
            if (SubjectClass.Contains(classNotificationInputDto.CourseClassID) == false)
            {
                return BadRequest(new { message = "Bạn không có quyền tạo thông báo cho lớp này." });
            }

            var data = new ClassNotifications
            {
                CourseClassID = classNotificationInputDto.CourseClassID,
                CreatedDate = DateTime.Now,
                CreatedBy = teacherId,
                Title = classNotificationInputDto.Title,
                Content = classNotificationInputDto.Content,
                IsImportant = classNotificationInputDto.IsImportant,
            };
            _context.Add(data);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok(new { message = "Tạo thông báo thành công" });
            }
            else
            {
                return BadRequest(new { message = "Tạo thông báo thất bại" });
            }
        }

        [HttpGet("TeacherGetClassNotification")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<ClassNotificationDto>>> TeacherGetClassNotification(string semesterID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();

            var data = await (from semesters in _context.Semesters
                              join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                              join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                              join room in _context.Rooms on courseClass.RoomID equals room.RoomID
                              join classPeriod in _context.ClassPeriods on courseClass.ClassPeriodID equals classPeriod.ClassPeriodID
                              join dayOfWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayOfWeek.DayofWeeksID
                              join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                              join department in _context.Departments on courseClass.DepartmentID equals department.DepartmentID
                              join classNotification in _context.ClassNotifications on courseClass.CourseClassID equals classNotification.CourseClassID
                              where teacher.TeacherID == teacherId && semesters.SemestersID == semesterID
                              select new ClassNotificationDto
                              {
                                  NotificationID = classNotification.NotificationID,
                                  CourseClassID = classNotification.CourseClassID,
                                  SubjectID = subject.SubjectsID,
                                  SubjectName = subject.SubjectsName,
                                  CreatedDate = classNotification.CreatedDate,
                                  CreatedBy = teacher.TeacherName,
                                  Title = classNotification.Title,
                                  Content = classNotification.Content,
                                  IsImportant = classNotification.IsImportant,
                                  IsDeleted = classNotification.IsDeleted,
                              }).Distinct().ToListAsync();
           

            if (data == null)
            {
                return BadRequest(new { message = "Không có thông báo nào." });
            }
            return Ok(data);
        }

        [HttpGet("StudentGetClassNotificaiton")]
        [Authorize("Student")]
        public async Task<ActionResult<List<ClassNotificationDto>>> StudentGetClassNotificaiton(string semesterId)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var notification = await (from notice in _context.ClassNotifications
                                      join courseclass in _context.CourseClass on notice.CourseClassID equals courseclass.CourseClassID
                                      join subject in _context.Subjects  on courseclass.SubjectsID equals subject.SubjectsID
                                      join teacher in _context.Teachers on courseclass.TeacherID equals teacher.TeacherID
                                      join enroll in _context.Enrolls on courseclass.Id equals enroll.CourseClass_ID
                                      join student in _context.Students on enroll.StudentID equals student.StudentID
                                      where student.UserID == userId && courseclass.SemestersID == semesterId && notice.IsDeleted == false
                                      select new ClassNotificationDto
                                      {
                                          NotificationID = notice.NotificationID,
                                          CourseClassID = courseclass.CourseClassID,
                                          SubjectID = subject.SubjectsID,
                                          SubjectName = subject.SubjectsName,
                                          CreatedDate = notice.CreatedDate,
                                          CreatedBy = teacher.TeacherName,
                                          Title = notice.Title,
                                          Content = notice.Content,
                                          IsDeleted = notice.IsDeleted,
                                          IsImportant = notice.IsImportant,
                                      }).Distinct().ToListAsync();
            if (notification == null)
            {
                return BadRequest("Không có thông báo nào.");
            }
            return Ok(notification);
        }

    }
}
