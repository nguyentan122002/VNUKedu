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
    public class NoticeOfLeavesController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public NoticeOfLeavesController(VNUK_DbContext context) 
        {
            _context = context;
         }

        [HttpGet("GetTeacherNoticeOfLeave")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<NoticeOfLeaveDto>>> GetTeacherNoticeOfLeave(string SemestersID)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID").Value);
                var teacherId = await _context.Teachers
                                              .Where(t => t.UserID == userId)
                                              .Select(t => t.TeacherID)
                                              .SingleOrDefaultAsync();

                var data = await (from notice in _context.NoticeOfLeaves
                                  join courseClass in _context.CourseClass on notice.CourseClassID equals courseClass.CourseClassID
                                  join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                  join semester in _context.Semesters on courseClass.SemestersID equals semester.SemestersID
                                  join teacher in _context.Teachers on notice.CreatedBy equals teacher.TeacherID
                                  join classperiod in _context.ClassPeriods on courseClass.ClassPeriodID equals classperiod.ClassPeriodID
                                  join dayofWeek in _context.DayofWeeks on classperiod.DayofWeeksID equals dayofWeek.DayofWeeksID
                                  where courseClass.SemestersID == SemestersID && notice.CreatedBy == teacherId && notice.IsDeleted == false
                                  select new NoticeOfLeaveDto
                                  {
                                      AdjustmentID = notice.AdjustmentID,
                                      CourseClassID = notice.CourseClassID,
                                      SubjectName = subject.SubjectsName,
                                      Reason = notice.Reason,
                                      CreatedBy = teacher.TeacherName,
                                      WeekOff = notice.WeekOff,
                                      ClassPeriodName = classperiod.ClassPeriodName,
                                      DayofWeekName = dayofWeek.DayofWeeksName

                                  }).Distinct().ToListAsync();

                var offleave = data.GroupBy(t => new
                {
                    t.AdjustmentID,
                    t.CourseClassID,
                    t.SubjectName,
                    t.Reason,
                    t.CreatedBy,
                    t.WeekOff,
                    t.DayofWeekName,
                }).Select(g => new NoticeOfLeaveDto
                {
                    AdjustmentID = g.Key.AdjustmentID,
                    CourseClassID = g.Key.CourseClassID,
                    SubjectName = g.Key.SubjectName,
                    Reason = g.Key.Reason,
                    CreatedBy = g.Key.CreatedBy,
                    WeekOff = g.Key.WeekOff,
                    ClassPeriodName = string.Join(",", g.Select(t => t.ClassPeriodName).Distinct()),
                    DayofWeekName = g.Key.DayofWeekName
                }).ToList();
                return Ok(offleave);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("GetStudentNoticeOfLeave")]
        [Authorize("Student")]
        public async Task<ActionResult<List<NoticeOfLeaveDto>>> GetStudentNoticeOfLeave(string SemestersID)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID").Value);
                var studentId = await _context.Students.Where(t => t.UserID == userId).Select(t => t.StudentID).SingleOrDefaultAsync();


                var data = await (from notice in _context.NoticeOfLeaves
                                  join courseClass in _context.CourseClass on notice.CourseClassID equals courseClass.CourseClassID
                                  join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                  join semester in _context.Semesters on courseClass.SemestersID equals semester.SemestersID
                                  join enroll in _context.Enrolls on courseClass.Id equals enroll.CourseClass_ID
                                  join teacher in _context.Teachers on notice.CreatedBy equals teacher.TeacherID
                                  join classperiod in _context.ClassPeriods on courseClass.ClassPeriodID equals classperiod.ClassPeriodID
                                  join dayofWeek in _context.DayofWeeks on classperiod.DayofWeeksID equals dayofWeek.DayofWeeksID
                                  where courseClass.SemestersID == SemestersID && enroll.StudentID == studentId && notice.IsDeleted == false
                                  select new NoticeOfLeaveDto
                                  {
                                      AdjustmentID = notice.AdjustmentID,
                                      CourseClassID = notice.CourseClassID,
                                      SubjectName = subject.SubjectsName,
                                      Reason = notice.Reason,
                                      CreatedBy = teacher.TeacherName,
                                      WeekOff = notice.WeekOff,
                                      ClassPeriodName = classperiod.ClassPeriodName,
                                      DayofWeekName = dayofWeek.DayofWeeksName
                                  }).Distinct().ToListAsync();
                var offleave = data.GroupBy(t => new
                {
                    t.AdjustmentID,
                    t.CourseClassID,
                    t.SubjectName,
                    t.Reason,
                    t.CreatedBy,
                    t.WeekOff,
                    t.DayofWeekName,
                }).Select(g => new NoticeOfLeaveDto
                {
                    AdjustmentID = g.Key.AdjustmentID,
                    CourseClassID = g.Key.CourseClassID,
                    SubjectName = g.Key.SubjectName,
                    Reason = g.Key.Reason,
                    CreatedBy = g.Key.CreatedBy,
                    WeekOff = g.Key.WeekOff,
                    ClassPeriodName = string.Join(",", g.Select(t => t.ClassPeriodName).Distinct()),
                    DayofWeekName = g.Key.DayofWeekName
                }).ToList();
                return Ok(offleave);

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpPost("CreateNoticeOfLeave")]
        [Authorize("Teacher")]
        public async Task<IActionResult> CreateNoticeOfLeave(NoticeOfLeaveInputDto noticeOfLeaveDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID").Value);

                var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();

                var subjectstart = await (from courseclass in _context.CourseClass
                                          join subject in _context.Subjects on courseclass.SubjectsID equals subject.SubjectsID
                                          where courseclass.CourseClassID == noticeOfLeaveDto.CourClassID
                                          select subject.WeekStart).Distinct().SingleOrDefaultAsync();

                var subjectend = await (from courseclass in _context.CourseClass
                                        join subject in _context.Subjects on courseclass.SubjectsID equals subject.SubjectsID
                                        where courseclass.CourseClassID == noticeOfLeaveDto.CourClassID
                                        select subject.WeekEnd).Distinct().SingleOrDefaultAsync();
                if (noticeOfLeaveDto.WeekOff < subjectstart || noticeOfLeaveDto.WeekOff > subjectend)
                {
                    return BadRequest("Tuần không hợp lệ");
                }

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
                if (SubjectClass.Contains(noticeOfLeaveDto.CourClassID) == false)
                {
                    return BadRequest(new { message = "Bạn không có quyền tạo thông báo cho lớp này." });
                }
                
                var data = new NoticeOfLeave
                {
                    CourseClassID = noticeOfLeaveDto.CourClassID,
                    Reason = noticeOfLeaveDto.Reason,
                    CreatedDate = DateTime.Now,
                    CreatedBy = teacherId,
                    WeekOff = noticeOfLeaveDto.WeekOff,
                };
                _context.Add(data);
                var result = _context.SaveChanges();

                if (result > 0)
                {
                    return Ok(new { message = "Tạo thông báo thành công" });
                }
                else
                {
                    return BadRequest(new { message = "Tạo thông báo thất bại" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("DeleteNoticeOfLeave")]
        [Authorize("Teacher")]
        public async Task<IActionResult> DeleteNoticeOfLeave(int adjustmentID)
        {
            var data = await _context.NoticeOfLeaves.SingleOrDefaultAsync(d => d.AdjustmentID == adjustmentID);

            data.IsDeleted = true;
            var result = _context.SaveChanges();
            if (result > 0)
            {
                return Ok(new { message = "Xóa thông báo thành công" });
            }
            else
            {
                return BadRequest(new { message = "Xóa thông báo thất bại" });
            }
        }

    }
}
