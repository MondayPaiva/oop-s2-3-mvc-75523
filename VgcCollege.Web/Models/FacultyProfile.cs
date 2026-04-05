using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class FacultyProfile
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}