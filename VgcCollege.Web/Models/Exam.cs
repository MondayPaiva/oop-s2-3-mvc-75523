using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, 100)]
        public int MaxScore { get; set; }

        public bool ResultsReleased { get; set; }

        public Course? Course { get; set; }

        public ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
    }
}