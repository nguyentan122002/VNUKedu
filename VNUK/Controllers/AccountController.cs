using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VNUK.Dtos.AccountDto;
using VNUK.Dtos.CommomDtos;
using VNUK.Dtos.UserDto;
using VNUK.Dtos.UserDtos.UserInput;
using VNUK.Models;
using VNUK.VNUKDbContext;

namespace VNUK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly VNUK_DbContext _context;

        public AccountController( VNUK_DbContext context, IConfiguration configuration) 
        { 
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => loginDto.UserName == u.UserName && loginDto.Password == u.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("UserID", user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var newAccessToken = CreateToken(authClaims);

            var token = new JwtSecurityTokenHandler().WriteToken(newAccessToken);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("GetInfoUser")]
        public IActionResult GetUserInfo() 
        {
            var userIdClaim = User.FindFirst("UserID")?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return Unauthorized("User not found");
            }

            int userId = int.Parse(userIdClaim);

            if (roleClaim == "Student") 
            {
                var student = _context.Students.Include(s => s.Users).Include(s => s.Majors).Include(s => s.Class).FirstOrDefault(s => s.UserID == userId);
                if (student != null)
                {
                    var studentResponse = new StudentDto
                    {
                        StudentID = student.StudentID,
                        StudentName = student.StudentName,
                        StudentEmail = student.Email,
                        StudentBirthDate = student.StudentDateOfBirth,
                        ClassName = student.Class.ClassName,
                        MajorName = student.Majors.MajorName,
                        ScoreEnglish = student.ScoreEnglish,
                        UserID = student.UserID,
                    };
                    return Ok(studentResponse );
                } 
                return Unauthorized("Khong tim thay sinh vien nay");
            }
            else if (roleClaim == "Teacher")
            {
                var teacher = _context.Teachers.Include(s => s.User).Include(t => t.Department).FirstOrDefault(t => t.UserID == userId);
                if (teacher != null)
                {
                    
                        var teacherresponse = new TeacherDto
                        {
                            TeacherID = teacher.TeacherID,
                            TeacherName = teacher.TeacherName,
                            IsCVHT = teacher.IsCVHT,
                            DepartmentName = teacher.Department.DepartmentName,
                            UserID = teacher.UserID,
                        };
                        return Ok(teacherresponse );
                    
                }
                return Unauthorized("Khong tim thay giang vien nay");
            }
            return Unauthorized("Khong tim thay nguoi dung");
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputDto changePasswordInputDto)
        {
            var userId = int.Parse(User.FindFirst("UserID").Value);

            if (changePasswordInputDto.Password != changePasswordInputDto.ConfirmPassword)
            {
                return BadRequest("Mật khẩu không chính xác");
            }
            else
            {
                var user = await _context.Users.Where(u => u.UserID == userId).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.Password = changePasswordInputDto.ConfirmPassword;
                    var result = _context.SaveChanges();
                    if (result > 0)
                    {
                        return Ok(new { message = "Cập nhật mật khẩu thành công." });
                    }
                    else
                    {
                        return BadRequest(new { message = "Cập nhật mật khẩu thất bại." });
                    }
                }
                return BadRequest(new { message = "Không tìm thấy người dùng." });
            }
           
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            _ = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
