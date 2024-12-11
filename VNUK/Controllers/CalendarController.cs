using DocumentFormat.OpenXml.InkML;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly VNUK_DbContext _context;

        public CalendarController(VNUK_DbContext context) 
        {
            _context = context;
        }

        [HttpPost("Create-event")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CreateEventCalendar(List<int> noticeOffsetID)
        {
            try
            {
                string[] Scopes = { "https://www.googleapis.com/auth/calendar" };
                string ApplicationName = "VNUK";
                UserCredential credential;
                using (var stream = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Credentials", "cre.json"), FileMode.Open, FileAccess.Read))
                {

                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    

                }

                var services = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                var attendees = new List<EventAttendee>();

                var email = await (from courseClass in _context.CourseClass
                                   join enroll in _context.Enrolls on courseClass.Id equals enroll.CourseClass_ID
                                   join student in _context.Students on enroll.StudentID equals student.StudentID
                                   join noticeoffset in _context.NoticeOffsets on courseClass.CourseClassID equals noticeoffset.CourseClassID
                                   where noticeOffsetID.Contains(noticeoffset.NoticeOffsetID)
                                   select student.Email).Distinct().ToListAsync();
                
                var attendeesList = email.Select(user => new EventAttendee { Email = user }).ToList();

                string note = await _context.NoticeOffsets.Where(t => noticeOffsetID.Contains(t.NoticeOffsetID)).Select(n => n.Note).Distinct().SingleOrDefaultAsync();

                string location = await _context.NoticeOffsets.Where(t => noticeOffsetID.Contains(t.NoticeOffsetID)).Select(l => l.RoomID).Distinct().SingleOrDefaultAsync();

                DateTime dateOffset = await _context.NoticeOffsets.Where(t => noticeOffsetID.Contains(t.NoticeOffsetID)).Select(s => s.DateOffset).Distinct().SingleOrDefaultAsync();

                var classperiodIds = await _context.NoticeOffsets.Where(t => noticeOffsetID.Contains(t.NoticeOffsetID)).Select(c => c.ClassPeriodID).Distinct().ToListAsync();
                var classPeriodMapping = new Dictionary<string, (TimeSpan startTime, TimeSpan endTime)>
                {
                    { "T1", (new TimeSpan(7, 0, 0), new TimeSpan(7, 50, 0)) },
                    { "T2", (new TimeSpan(8, 0, 0), new TimeSpan(8, 50, 0)) },
                    { "T3", (new TimeSpan(9, 0, 0), new TimeSpan(9, 50, 0)) },
                    { "T4", (new TimeSpan(10, 0, 0), new TimeSpan(10, 50, 0)) },
                    { "T5", (new TimeSpan(11, 0, 0), new TimeSpan(11, 50, 0)) },
                    { "T6", (new TimeSpan(12, 0, 0), new TimeSpan(12, 50, 0)) },
                    { "T7", (new TimeSpan(13, 0, 0), new TimeSpan(13, 50, 0)) },
                    { "T8", (new TimeSpan(14, 0, 0), new TimeSpan(14, 50, 0)) },
                    { "T9", (new TimeSpan(15, 0, 0), new TimeSpan(15, 50, 0)) },
                    { "T10", (new TimeSpan(16, 0, 0), new TimeSpan(16, 50, 0)) },

                };
                string firstPeriodId = classperiodIds.First();
                string lastPeriodId = classperiodIds.Last();

                classPeriodMapping.TryGetValue(firstPeriodId.Substring(0, 2), out var startTimes);
                classPeriodMapping.TryGetValue(lastPeriodId.Substring(0, 2), out var endTimes);

                DateTime startDateTime = dateOffset.Date + startTimes.startTime;
                DateTime endDateTime = dateOffset.Date + endTimes.endTime;

                Event eventCalendar = new Event()
                {
                    Summary = "HỌC BÙ",
                    Location = location,
                    Start = new EventDateTime
                    {
                        DateTime = startDateTime,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    End = new EventDateTime
                    {
                        DateTime = endDateTime,
                        TimeZone = "Asia/Ho_Chi_Minh"
                    },
                    Description = note,
                    Attendees = attendeesList
                };

                var eventRequest = services.Events.Insert(eventCalendar, "primary");
                
                var requestCreate = await eventRequest.ExecuteAsync();


                return Ok(requestCreate);

            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
