using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VNUK.Models;
using VNUK.Models.Exam;

namespace VNUK.VNUKDbContext
{
    public class VNUK_DbContext : DbContext
    {
        public VNUK_DbContext(DbContextOptions options): base(options) { }

        public DbSet<Roles> Roles { get; set; }
        
        public DbSet<Classes> Class { get; set; }
        
        public DbSet<ClassPeriod> ClassPeriods { get; set; }
        
        public DbSet<CourseClasses> CourseClass { get; set; }
        
        public DbSet<DayofWeeks> DayofWeeks { get; set; }
        
        public DbSet<Departments> Departments { get; set; }
        
        public DbSet<Enroll> Enrolls { get; set; }
        
        public DbSet<Majors> Majors { get; set; }
        
        public DbSet<Permission> Permissions { get; set; }
        
        public DbSet<RolePermission> RolePermissions { get; set; }
        
        public DbSet<Rooms> Rooms { get; set; }
        
        public DbSet<Semesters> Semesters { get; set; }
        
        public DbSet<Students> Students { get; set; }
        
        public DbSet<Subjects> Subjects { get; set; }
        
        public DbSet<Teachers> Teachers { get; set; }
        
        public DbSet<Users> Users {  get; set; }
        
        public DbSet<ClassNotifications> ClassNotifications { get; set; }
        
        public DbSet<NoticeOfLeave> NoticeOfLeaves { get; set; }
        
        public DbSet<NoticeOffset> NoticeOffsets { get; set; }
        
        public DbSet<RoomBooking> RoomBookings { get; set; }
        
        public DbSet<SessionChangeLog> SessionChangeLogs { get; set; }
        
        public DbSet<ExamRoomAllocations> ExamRoomAllocations { get; set; }
        
        public DbSet<Exams> Exams { get; set; }

        public DbSet<ExamTypes> ExamTypes { get; set; }

        public DbSet<Proctors> Proctors { get; set; }

        public DbSet<StudentExams> StudentExams { get; set; }

        public DbSet<RoomBooking> RoomBooking { get; set; }
    }
}
