using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class CourseEnrolment
    {
        public int Id { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [DataType(DataType.Date)]
        public DateTime EnrolDate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public StudentProfile? StudentProfile { get; set; }
        public Course? Course { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}