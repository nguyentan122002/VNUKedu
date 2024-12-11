using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.CommomDtos;
using VNUK.Dtos.Notices.NoticeInput;
using VNUK.Dtos.RoomBookingDto;
using VNUK.Dtos.RoomBookingDtos;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomBookingController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public RoomBookingController(VNUK_DbContext context) 
        {
            _context = context;
        }

        [HttpGet("GetRoomBooking")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<RoomBookingDto>>> GetRoomBooking()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID").Value);

                var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();

                var booking = await (from roombooking in _context.RoomBooking
                                     join room in _context.Rooms on roombooking.RoomID equals room.RoomID
                                     join teacher in _context.Teachers on roombooking.BookedBy equals teacher.TeacherID
                                     join classPeriod in _context.ClassPeriods on roombooking.ClassPeriodID equals classPeriod.ClassPeriodID
                                     join dayofweek in _context.DayofWeeks on classPeriod.DayofWeeksID equals dayofweek.DayofWeeksID
                                     where teacher.TeacherID == teacherId
                                     select new RoomBookingDto
                                     {
                                         BookingID = roombooking.BookingID,
                                         RoomID = roombooking.RoomID,
                                         RoomName = room.RoomName,
                                         WeekBooking = roombooking.WeekBooking,
                                         ClassPeriodName = classPeriod.ClassPeriodName,
                                         DayofWeekname = dayofweek.DayofWeeksName,
                                         Purpose = roombooking.Purpose,
                                         CreatedDate = roombooking.CreatedDate,
                                         BookedBy = teacher.TeacherName
                                     }).ToListAsync();
                
                if (!booking.Any())
                {
                    return NotFound("Không tìm thấy lịch đặt phòng nào của bạn.");
                }
                return Ok(booking);
            }
            catch  
            {
                return BadRequest("Đã xảy ra lỗi.");
            }
        }

        [HttpPost("CreateBookingRoom")]
        [Authorize("Teacher")]
        public async Task<IActionResult> CreateBookingRoom(RoomBookingInputDto roomBookingInputDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserID").Value);

                var teacherId = await _context.Teachers.Where(t => t.UserID == userId).Select(t => t.TeacherID).SingleOrDefaultAsync();


                var notices = new List<RoomBooking>();
                for (int i = 0; i < roomBookingInputDto.ClassPeriodIDs.Count; i++)
                {
                    var classPeriodIDs = roomBookingInputDto.ClassPeriodIDs[i];

                    notices.Add(new RoomBooking
                    {
                        RoomID = roomBookingInputDto.RoomID,
                        WeekBooking = roomBookingInputDto.WeekBooking,
                        ClassPeriodID = classPeriodIDs,
                        Purpose = roomBookingInputDto.Purpose,
                        CreatedDate = DateTime.Now,
                        BookedBy = teacherId,
                    });
                }

                _context.RoomBooking.AddRange(notices);
                var result = await _context.SaveChangesAsync();
                if(result > 0)
                {
                    return Ok("Đã đăng ký phòng thành công.");
                }
                return BadRequest("Đăng ký phòng thất bại.");
            }
            catch 
            {
                return BadRequest("Đã xảy ra lỗi.");
            }
        }
        
        [HttpPost("GetAvailableRoom")]
        [Authorize("Teacher")]
        public async Task<ActionResult<List<RoomDto>>> GetAvailableRoom(string semesterId, List<string> classPeriodId, int weekBooking)
        {
            var occupiedRoomIDs = await _context.CourseClass
                .Where(c => classPeriodId.Contains(c.ClassPeriodID)
                            && c.SemestersID == semesterId
                            && !_context.NoticeOfLeaves
                                .Where(n => n.CourseClassID == c.CourseClassID && n.WeekOff == weekBooking && n.IsDeleted == false)
                                .Select(n => n.CourseClassID)
                                .Contains(c.CourseClassID)
                            )
                .Select(c => c.RoomID)
                .Distinct()
                .ToListAsync();

            var OffsetRoomIDs = await _context.NoticeOffsets
                .Where(n => classPeriodId.Contains(n.ClassPeriodID) && n.WeekOffset == weekBooking)
                .Select(n => n.RoomID)
                .Distinct()
                .ToListAsync();

            var availableRoomIDs = _context.Rooms
               .Where(r => !occupiedRoomIDs.Contains(r.RoomID) && !OffsetRoomIDs.Contains(r.RoomID))
               .Select(r => r.RoomID);

            var data = await _context.Rooms
                .Where(r => availableRoomIDs.Contains(r.RoomID))
                .Select(r => new RoomDto
                {
                    RoomID = r.RoomID,
                    RoomName = r.RoomName,
                    NumberOfSeat = r.NumberOfSeats,
                    RoomType = r.RoomType
                })
                .ToListAsync();

            if (data == null)
            {
                return NotFound("Không tìm thấy phòng trống.");
            }
            return Ok(data);
                                          
        }


    }
}
