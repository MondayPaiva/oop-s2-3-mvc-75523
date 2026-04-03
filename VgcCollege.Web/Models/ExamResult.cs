using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class ExamResult
    {
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        public string Grade { get; set; } = string.Empty;

        public Exam? Exam { get; set; }
        public StudentProfile? StudentProfile { get; set; }
    }
}