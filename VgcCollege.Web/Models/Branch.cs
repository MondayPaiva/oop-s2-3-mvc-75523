using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}