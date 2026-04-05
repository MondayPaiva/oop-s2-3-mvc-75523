using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext(options)
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<FacultyProfile> FacultyProfiles { get; set; }
        public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentResult> AssignmentResults { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Course>()
                .HasOne(c => c.Branch)
                .WithMany(b => b.Courses)
                .HasForeignKey(c => c.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Course>()
                .HasOne(c => c.FacultyProfile)
                .WithMany(f => f.Courses)
                .HasForeignKey(c => c.FacultyProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CourseEnrolment>()
                .HasOne(e => e.StudentProfile)
                .WithMany(s => s.Enrolments)
                .HasForeignKey(e => e.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseEnrolment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttendanceRecord>()
                .HasOne(a => a.CourseEnrolment)
                .WithMany(e => e.AttendanceRecords)
                .HasForeignKey(a => a.CourseEnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssignmentResult>()
                .HasOne(ar => ar.Assignment)
                .WithMany(a => a.Results)
                .HasForeignKey(ar => ar.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AssignmentResult>()
                .HasOne(ar => ar.StudentProfile)
                .WithMany(s => s.AssignmentResults)
                .HasForeignKey(ar => ar.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Exam>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamResult>()
                .HasOne(er => er.Exam)
                .WithMany(e => e.Results)
                .HasForeignKey(er => er.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamResult>()
                .HasOne(er => er.StudentProfile)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(er => er.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentProfile>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();
        }
    }
}