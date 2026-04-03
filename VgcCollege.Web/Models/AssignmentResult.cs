using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class AssignmentResult
    {
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        public int StudentProfileId { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }

        public string Feedback { get; set; } = string.Empty;

        public Assignment? Assignment { get; set; }
        public StudentProfile? StudentProfile { get; set; }
    }
}