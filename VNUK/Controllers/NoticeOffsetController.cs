using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VNUK.Dtos.CommomDtos;
using VNUK.Dtos.Notices;
using VNUK.Dtos.Notices.NoticeInput;
using VNUK.Models;
using VNUK.VNUKDbContext;
using VNUK.Dtos;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.InkML;
using System;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeOffsetController : ControllerBase
    {
        private readonly string[] Scopes = { CalendarService.Scope.Calendar };
        private readonly VNUK_DbContext _context;

        public NoticeOffsetController(VNUK_DbContext context) 
        {
            _context = context;
        }


        [HttpPost("GetAvailableRooms")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<RoomDto>>> GetAvailableRooms(string courseClassId ,List<string> classPeriodIDs,  int weekOffset, string semesterId)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var SubjectClass = await (from semesters in _context.Semesters
                                      join courseClass in _context.CourseClass on semesters.SemestersID equals courseClass.SemestersID
                                      join teacher in _context.Teachers on courseClass.TeacherID equals teacher.TeacherID
                                      join room in _context.Rooms on courseClass.RoomID equals room.RoomID
                                      join classPeriod in _context.ClassPeriods on courseClass.ClassPeriodID equals classPeriod.ClassPeriodID
                                      join dayOfWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayOfWeek.DayofWeeksID
                                      join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                      join department in _context.Departments on courseClass.DepartmentID equals department.DepartmentID
                                      where teacher.UserID == userId
                                      select courseClass.CourseClassID).Distinct().ToListAsync();
            if (SubjectClass.Contains(courseClassId) == false)
            {
                return BadRequest("Bạn không có quyền tra cứu thông tin cho lớp này.");
            }

            var subjectstart = await (from courseclass in _context.CourseClass
                                      join subject in _context.Subjects on courseclass.SubjectsID equals subject.SubjectsID
                                      where courseclass.CourseClassID == courseClassId && courseclass.SemestersID == semesterId
                                      select subject.WeekStart).Distinct().SingleOrDefaultAsync();


            var subjectend = await (from courseclass in _context.CourseClass
                                      join subject in _context.Subjects on courseclass.SubjectsID equals subject.SubjectsID
                                      where courseclass.CourseClassID == courseClassId && courseclass.SemestersID == semesterId
                                      select subject.WeekEnd).Distinct().SingleOrDefaultAsync();

            var semesterend = await _context.Semesters.Where(s => s.SemestersID == semesterId).Select(s => s.WeekEnd).SingleOrDefaultAsync();

            if (weekOffset < subjectstart || weekOffset > semesterend)
            {
                return BadRequest("Tuần không hợp lệ");
            }
            else if (weekOffset > subjectend)
            {
                var occupiedRoom = await _context.CourseClass
                    .Where(c => _context.NoticeOffsets
                           .Where(n => n.CourseClassID == c.CourseClassID && n.WeekOffset == weekOffset && n.IsDeleted == false)
                           .Select(n=> n.CourseClassID).Contains(c.CourseClassID))
                    .Select(n => n.RoomID)
                    .Distinct()
                    .ToListAsync();

                var availableRoomID = _context.Rooms.Where(r => !occupiedRoom.Contains(r.RoomID)).Select(r => r.RoomID);

                var room = await _context.Rooms.Where(r => availableRoomID.Contains(r.RoomID)).Select(r => new RoomDto
                {
                    RoomID = r.RoomID,
                    RoomName = r.RoomName,
                    NumberOfSeat = r.NumberOfSeats,
                    RoomType = r.RoomType,
                }).ToListAsync();

                return Ok(room);
            }
            else
            {
                var occupiedRoomIDs = await _context.CourseClass
                .Where(c => classPeriodIDs.Contains(c.ClassPeriodID)
                            && c.SemestersID == semesterId
                            && !_context.NoticeOfLeaves
                                .Where(n => n.CourseClassID == c.CourseClassID && n.WeekOff == weekOffset && n.IsDeleted == false)
                                .Select(n => n.CourseClassID)
                                .Contains(c.CourseClassID)
                            )
                .Select(c => c.RoomID)
                .Distinct()
                .ToListAsync();

                var OffsetRoomIDs = await _context.NoticeOffsets
                    .Where(n => classPeriodIDs.Contains(n.ClassPeriodID) && n.WeekOffset == weekOffset)
                    .Select(n => n.RoomID)
                    .Distinct()
                    .ToListAsync();

                var availableRoomIDs = _context.Rooms
                   .Where(r => !occupiedRoomIDs.Contains(r.RoomID) && !OffsetRoomIDs.Contains(r.RoomID))
                   .Select(r => r.RoomID);

                var availableRooms = await _context.Rooms
                    .Where(r => availableRoomIDs.Contains(r.RoomID))
                    .Select(r => new RoomDto
                    {
                        RoomID = r.RoomID,
                        RoomName = r.RoomName,
                        NumberOfSeat = r.NumberOfSeats,
                        RoomType = r.RoomType
                    })
                    .ToListAsync();


                if (!availableRooms.Any())
                {
                    return NotFound("Không có phòng trống trong thời gian bạn chọn.");
                }

                return Ok(availableRooms);
            }
            
        }


        [HttpGet("GetListClassPeriod")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<ClassPeriodDto>>> GetListClassPeriod(string dayofWeekID)
        {
            var data = await _context.ClassPeriods.Where(c => c.DayofWeeksID == dayofWeekID).Select(c => new ClassPeriodDto
            {
                ClassPeriodID = c.ClassPeriodID,
                ClassPeriodName = c.ClassPeriodName,
                DayofWeekName = c.DayofWeeks.DayofWeeksName,
            }).ToListAsync();
            if (data == null)
            {
                return BadRequest("Không tìm thấy tiết học");
            }
            return Ok(data);
        }

        [HttpGet("GetListDayofWeeks")]
        [Authorize("Teacher")]
        public async Task<ActionResult<DayofWeekDto>> GetListDayofWeeks()
        {
            var data = await _context.DayofWeeks.Select(d => new DayofWeekDto
            {
                DayofWeeksID = d.DayofWeeksID,
                DayofWeeksName = d.DayofWeeksName,
            }).ToListAsync();
            if (data == null)
            {
                return Unauthorized("Không tìm thấy thứ");
            }
            return Ok(data);
        }


        [HttpPost("CreateNoticeOffset")]
        [Authorize("Teacher")]
        public async Task<IActionResult> CreateNoticeOffset(NoticeOffsetInputDto noticeOffsetInputDto)
        {
            try
            {
                var dateStartSemester = new DateTime(2024, 07, 29);
                var userId = int.Parse(User.FindFirst("UserID").Value);
                var teacherId = await _context.Teachers
                    .Where(t => t.UserID == userId)
                    .Select(t => t.TeacherID)
                    .SingleOrDefaultAsync();

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
                if (SubjectClass.Contains(noticeOffsetInputDto.CourseClassID) == false)
                {
                    return BadRequest(new { message = "Bạn không có quyền tạo thông báo cho lớp này." });
                }
                

                var notices = new List<NoticeOffset>();
                for (int i = 0; i < noticeOffsetInputDto.ClassPeriodIDs.Count; i++)
                {
                    var classPeriodID = noticeOffsetInputDto.ClassPeriodIDs[i];

                    var dayofweek = await(from classPeriod in _context.ClassPeriods 
                                        join dayOfWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayOfWeek.DayofWeeksID
                                    where classPeriod.ClassPeriodID == classPeriodID
                                    select dayOfWeek.DayofWeeksName).Distinct().SingleOrDefaultAsync();

                    int getDateOffset = dayofweek switch
                    {
                        "Thứ 2" => 0,
                        "Thứ 3" => 1,
                        "Thứ 4" => 2,
                        "Thứ 5" => 3,
                        "Thứ 6" => 4,
                        "Thứ 7" => 5,
                        "Chủ nhật" => 6,
                        _ => throw new Exception("Thứ không đúng.")
                    };

                    DateTime startWeek = dateStartSemester.AddDays((noticeOffsetInputDto.WeekOffset - 1) * 7);
                    DateTime dateOffset = startWeek.AddDays(getDateOffset);

                    notices.Add(new NoticeOffset
                    {
                        CourseClassID = noticeOffsetInputDto.CourseClassID ,
                        ClassPeriodID = classPeriodID,
                        RoomID = noticeOffsetInputDto.RoomID,
                        WeekOffset = noticeOffsetInputDto.WeekOffset,
                        Note = noticeOffsetInputDto.Note,
                        CreatedBy = teacherId,
                        CreatedDate = DateTime.Now,
                        DateOffset = dateOffset,
                    });
                }

                _context.NoticeOffsets.AddRange(notices);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return Ok("Tạo thông báo thành công.");
                }
                else
                {
                    return BadRequest( "Tạo thông báo thất bại." );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("GetTeacherNoticeOffset")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<NoticeOffsetDto>>> GetTeacherNoticeOffset(string semestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();

            var notice = await (from noticeOffset in _context.NoticeOffsets
                                join courseClass in _context.CourseClass on noticeOffset.CourseClassID equals courseClass.CourseClassID
                                join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                join classPeriod in _context.ClassPeriods on noticeOffset.ClassPeriodID equals classPeriod.ClassPeriodID
                                join dayofWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayofWeek.DayofWeeksID
                                join room in _context.Rooms on noticeOffset.RoomID equals room.RoomID
                                join teacher in _context.Teachers on noticeOffset.CreatedBy equals teacher.TeacherID
                                join semester in _context.Semesters on courseClass.SemestersID equals semester.SemestersID
                                where teacher.TeacherID == teacherId && semester.SemestersID == semestersID && noticeOffset.IsDeleted == false
                                select new NoticeOffsetDto
                                {
                                    NoticeOffsetID = noticeOffset.NoticeOffsetID.ToString(),
                                    CourseClassID = courseClass.CourseClassID,
                                    SubjectID = subject.SubjectsID,
                                    SubjectName = subject.SubjectsName,
                                    CreatedBy = teacher.TeacherName,
                                    ClassPeriodname = classPeriod.ClassPeriodName,
                                    DayofWeekName = dayofWeek.DayofWeeksName,
                                    RoomName = room.RoomName,
                                    WeekOffset = noticeOffset.WeekOffset,
                                    Note = noticeOffset.Note,
                                    DateOffset = noticeOffset.DateOffset,
                                }).ToListAsync();

            var data = notice
                .GroupBy(n => new
                {
                    n.CourseClassID,
                    n.SubjectID,
                    n.SubjectName,
                    n.CreatedBy,
                    n.DayofWeekName,
                    n.RoomName,
                    n.WeekOffset,
                    n.Note,
                    n.DateOffset
                })
                .Select(g => new NoticeOffsetDto
                {
                    NoticeOffsetID = string.Join(",", g.Select(t => t.NoticeOffsetID).Distinct()),
                    CourseClassID = g.Key.CourseClassID,
                    SubjectID = g.Key.SubjectID,
                    SubjectName = g.Key.SubjectName,
                    CreatedBy = g.Key.CreatedBy,
                    ClassPeriodname = string.Join(",", g.Select(t => t.ClassPeriodname).Distinct()),
                    DayofWeekName = g.Key.DayofWeekName,
                    RoomName = g.Key.RoomName,
                    WeekOffset = g.Key.WeekOffset,
                    Note = g.Key.Note,
                    DateOffset = g.Key.DateOffset,
                })
                .ToList();

            return Ok(data);
        }


        [HttpGet("GetStudentNoticeOffset")]
        [Authorize("Student")]
        public async Task<ActionResult<List<NoticeOffsetDto>>> GetStudentNoticeOffset(string semestersID)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            var studentId = await _context.Students.Where(s => s.UserID == userId).Select(s=>s.StudentID).SingleOrDefaultAsync();

            var notice = await (from noticeOffset in _context.NoticeOffsets
                                join courseClass in _context.CourseClass on noticeOffset.CourseClassID equals courseClass.CourseClassID
                                join subject in _context.Subjects on courseClass.SubjectsID equals subject.SubjectsID
                                join classPeriod in _context.ClassPeriods on noticeOffset.ClassPeriodID equals classPeriod.ClassPeriodID
                                join dayofWeek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayofWeek.DayofWeeksID
                                join room in _context.Rooms on noticeOffset.RoomID equals room.RoomID
                                join teacher in _context.Teachers on noticeOffset.CreatedBy equals teacher.TeacherID
                                join enroll in _context.Enrolls on courseClass.Id equals enroll.CourseClass_ID
                                join student in _context.Students on enroll.StudentID equals student.StudentID
                                join semester in _context.Semesters on courseClass.SemestersID equals semester.SemestersID
                                where student.StudentID == studentId && semester.SemestersID == semestersID && noticeOffset.IsDeleted == false
                                select new NoticeOffsetDto
                                {
                                    NoticeOffsetID = noticeOffset.NoticeOffsetID.ToString(),
                                    CourseClassID = courseClass.CourseClassID,
                                    SubjectID = subject.SubjectsID,
                                    SubjectName = subject.SubjectsName,
                                    CreatedBy = teacher.TeacherName,
                                    ClassPeriodname = classPeriod.ClassPeriodName,
                                    DayofWeekName = dayofWeek.DayofWeeksName,
                                    RoomName = room.RoomName,
                                    WeekOffset = noticeOffset.WeekOffset,
                                    Note = noticeOffset.Note,
                                    DateOffset = noticeOffset.DateOffset,
                                }).ToListAsync();
            var data = notice.GroupBy(n => new
            {
                n.CourseClassID,
                n.SubjectID,
                n.SubjectName,
                n.CreatedBy,
                n.DayofWeekName,
                n.RoomName,
                n.WeekOffset,
                n.Note,
                n.DateOffset

            }).Select(g => new NoticeOffsetDto
            {
                NoticeOffsetID = string.Join(",", g.Select(t => t.NoticeOffsetID).Distinct()),
                CourseClassID = g.Key.CourseClassID,
                SubjectID = g.Key.SubjectID,
                SubjectName = g.Key.SubjectName,
                CreatedBy = g.Key.CreatedBy,
                ClassPeriodname = string.Join(",", g.Select(t => t.ClassPeriodname).Distinct()),
                DayofWeekName = g.Key.DayofWeekName,
                RoomName = g.Key.RoomName,
                WeekOffset = g.Key.WeekOffset,
                Note = g.Key.Note,
                DateOffset = g.Key.DateOffset,
            }).ToList();
            return Ok(data);
        }

        [HttpPut("DeleteNoticeOffset")]
        [Authorize("Teacher")]
        public async Task<IActionResult> DeleteNoticeOffset([FromBody] List<int> noticeOffset)
        {
            var noticeOffsets = await _context.NoticeOffsets
                                        .Where(no => noticeOffset.Contains(no.NoticeOffsetID) && no.IsDeleted == false)
                                        .ToListAsync();

            if (noticeOffsets == null || !noticeOffsets.Any())
            {
                return NotFound("Không tìm thấy Thông báo.");
            }

            foreach (var Offset in noticeOffsets)
            {
                Offset.IsDeleted = true;
            }
            _context.NoticeOffsets.UpdateRange(noticeOffsets);
            var result = _context.SaveChanges();
            if (result > 0)
            {
                return Ok( "Xóa thông báo thành công" );
            }
            else
            {
                return BadRequest( "Xóa thông báo thất bại" );
            }
        }

    }
}
