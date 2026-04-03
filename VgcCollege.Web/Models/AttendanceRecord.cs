using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }

        [Required]
        public int CourseEnrolmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        public bool Present { get; set; }

        public CourseEnrolment? CourseEnrolment { get; set; }
    }
}