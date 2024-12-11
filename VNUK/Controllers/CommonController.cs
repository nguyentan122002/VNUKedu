using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VNUK.Dtos.CommomDtos;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly VNUK_DbContext _context;
        public CommonController(VNUK_DbContext context) 
        {
            _context = context;
        }

        [HttpGet("GetSemester")]
        [Authorize]
        public async Task<ActionResult<List<SemesterDto>>> GetSemester()
        {
            var data = await _context.Semesters.Select(s => new SemesterDto
            {
                SemestersID = s.SemestersID,
                SemestersName = s.SemestersName,
                WeekStart = s.WeekStart,
                WeekEnd = s.WeekEnd,
            }).ToListAsync();

            if (data == null)
            {
                return Unauthorized("Không tìm thấy học kỳ nào");
            }
            return Ok(data);
        }

        [HttpPost("ImportSemester")]
        [Authorize("Teacher")]
        public async Task<IActionResult> ImportSemester(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var semesters = ParseExcelFile(file.OpenReadStream());
                semesters = semesters.DistinctBy(c => c.SemestersID).ToList();
                await _context.Semesters.AddRangeAsync(semesters);
                await _context.SaveChangesAsync();

                return Ok("Import thành công");
            }
            return BadRequest("File không đúng hoặc không có dữ liệu. Vui lòng kiểm tra lại file.");
        }


        private List<Semesters> ParseExcelFile(Stream stream)
        {
            var semesters = new List<Semesters>();

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var data = new Semesters
                    {
                        SemestersID = row.Cell(1).GetValue<string>(),
                        SemestersName = row.Cell(2).GetValue<string>(),
                        WeekStart = row.Cell(3).GetValue<int>(),
                        WeekEnd = row.Cell(4).GetValue<int>(),
                    };
                    semesters.Add(data);
                }
            }
            return semesters;
        }
    }
}
