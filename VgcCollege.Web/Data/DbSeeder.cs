using Microsoft.AspNetCore.Identity;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesUsersAndDataAsync(IServiceProvider services, ApplicationDbContext context)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = await CreateUser(userManager, "admin@vgc.com", "Admin123!", "Admin");
            var facultyUser = await CreateUser(userManager, "faculty@vgc.com", "Admin123!", "Faculty");
            var studentUser1 = await CreateUser(userManager, "student1@vgc.com", "Admin123!", "Student");
            var studentUser2 = await CreateUser(userManager, "student2@vgc.com", "Admin123!", "Student");

            if (context.Branches.Any())
                return;

            var branches = new List<Branch>
            {
                new() { Name = "Dublin City Branch", Address = "Dublin City Centre" },
                new() { Name = "Cork Branch", Address = "Cork Main Street" },
                new() { Name = "Galway Branch", Address = "Galway Central" }
            };

            context.Branches.AddRange(branches);
            await context.SaveChangesAsync();

            var facultyProfile = new FacultyProfile
            {
                IdentityUserId = facultyUser.Id,
                Name = "Jane Faculty",
                Email = facultyUser.Email!,
                Phone = "0850000001"
            };

            context.FacultyProfiles.Add(facultyProfile);
            await context.SaveChangesAsync();

            var courses = new List<Course>
            {
                new()
                {
                    Name = "Computing Fundamentals",
                    BranchId = branches[0].Id,
                    FacultyProfileId = facultyProfile.Id,
                    StartDate = DateTime.Today.AddDays(-30),
                    EndDate = DateTime.Today.AddMonths(6)
                },
                new()
                {
                    Name = "Business Studies",
                    BranchId = branches[1].Id,
                    FacultyProfileId = facultyProfile.Id,
                    StartDate = DateTime.Today.AddDays(-20),
                    EndDate = DateTime.Today.AddMonths(6)
                },
                new()
                {
                    Name = "Digital Marketing",
                    BranchId = branches[2].Id,
                    FacultyProfileId = facultyProfile.Id,
                    StartDate = DateTime.Today.AddDays(-10),
                    EndDate = DateTime.Today.AddMonths(6)
                }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            var student1 = new StudentProfile
            {
                IdentityUserId = studentUser1.Id,
                Name = "Student One",
                Email = studentUser1.Email!,
                Phone = "0850000002",
                Address = "Dublin",
                StudentNumber = "STU1001"
            };

            var student2 = new StudentProfile
            {
                IdentityUserId = studentUser2.Id,
                Name = "Student Two",
                Email = studentUser2.Email!,
                Phone = "0850000003",
                Address = "Cork",
                StudentNumber = "STU1002"
            };

            context.StudentProfiles.AddRange(student1, student2);
            await context.SaveChangesAsync();

            var enrolments = new List<CourseEnrolment>
            {
                new()
                {
                    StudentProfileId = student1.Id,
                    CourseId = courses[0].Id,
                    EnrolDate = DateTime.Today.AddDays(-25),
                    Status = "Active"
                },
                new()
                {
                    StudentProfileId = student2.Id,
                    CourseId = courses[0].Id,
                    EnrolDate = DateTime.Today.AddDays(-22),
                    Status = "Active"
                }
            };

            context.CourseEnrolments.AddRange(enrolments);
            await context.SaveChangesAsync();

            var attendance = new List<AttendanceRecord>
            {
                new() { CourseEnrolmentId = enrolments[0].Id, SessionDate = DateTime.Today.AddDays(-14), Present = true },
                new() { CourseEnrolmentId = enrolments[0].Id, SessionDate = DateTime.Today.AddDays(-7), Present = true },
                new() { CourseEnrolmentId = enrolments[1].Id, SessionDate = DateTime.Today.AddDays(-14), Present = false },
                new() { CourseEnrolmentId = enrolments[1].Id, SessionDate = DateTime.Today.AddDays(-7), Present = true }
            };

            context.AttendanceRecords.AddRange(attendance);
            await context.SaveChangesAsync();

            var assignment = new Assignment
            {
                CourseId = courses[0].Id,
                Title = "C# Basics Assignment",
                MaxScore = 100,
                DueDate = DateTime.Today.AddDays(7)
            };

            context.Assignments.Add(assignment);
            await context.SaveChangesAsync();

            var assignmentResults = new List<AssignmentResult>
            {
                new() { AssignmentId = assignment.Id, StudentProfileId = student1.Id, Score = 78, Feedback = "Good work" },
                new() { AssignmentId = assignment.Id, StudentProfileId = student2.Id, Score = 65, Feedback = "Needs more detail" }
            };

            context.AssignmentResults.AddRange(assignmentResults);
            await context.SaveChangesAsync();

            var exam = new Exam
            {
                CourseId = courses[0].Id,
                Title = "Midterm Exam",
                Date = DateTime.Today.AddDays(14),
                MaxScore = 100,
                ResultsReleased = false
            };

            context.Exams.Add(exam);
            await context.SaveChangesAsync();

            var examResults = new List<ExamResult>
            {
                new() { ExamId = exam.Id, StudentProfileId = student1.Id, Score = 82, Grade = "B" },
                new() { ExamId = exam.Id, StudentProfileId = student2.Id, Score = 71, Grade = "C" }
            };

            context.ExamResults.AddRange(examResults);
            await context.SaveChangesAsync();
        }

        private static async Task<IdentityUser> CreateUser(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {email}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}