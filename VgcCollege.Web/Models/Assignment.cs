using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Range(0, 100)]
        public int MaxScore { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public Course? Course { get; set; }

        public ICollection<AssignmentResult> Results { get; set; } = new List<AssignmentResult>();
    }
}