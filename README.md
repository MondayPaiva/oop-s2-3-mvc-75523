# VGC College Web

VGC College Web is an ASP.NET Core MVC academic management system developed to simulate a real college/university administrative platform.

The project was designed to manage key academic entities such as branches, courses, students, enrolments, attendance, assignments, exams, and results, while also applying role-based access control for different users.

---

## Features

- Branch management
- Course management
- Student profile management
- Course enrolments
- Attendance records
- Assignments and assignment results
- Exams and exam results
- Role-based access control
- Professional academic-style user interface

---

## User Roles

The system includes three main roles:

### Admin
The admin has full access to the system and can manage all entities.

### Faculty
Faculty users can only access courses, assignments, exams, and related results connected to their own courses.

### Student
Students can only access their own profile, enrolments, assignment results, and exam results.  
They are not allowed to see other students’ data.

---

## Security and Access Rules

Some important access rules implemented in the project:

- Students can only view their own data
- Faculty can only view data linked to their assigned courses
- Admin has full control of the platform
- Exam results are only visible to students after they are officially released
- Unauthorized users are blocked from restricted pages

---

## Technologies Used

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQLite
- ASP.NET Core Identity
- Bootstrap
- Custom CSS

---

## Project Structure

The project follows the MVC pattern:

- **Models**: represent the academic entities and relationships
- **Views**: user interface pages
- **Controllers**: business rules, permissions, and application flow

---

## Default Test Accounts

You can use the following accounts for testing:

### Admin
- Email: `admin@vgc.com`
- Password: `Admin123!`

### Faculty
- Email: `faculty@vgc.com`
- Password: `Admin123!`

### Student 1
- Email: `student1@vgc.com`
- Password: `Admin123!`

### Student 2
- Email: `student2@vgc.com`
- Password: `Admin123!`

---

## How to Run the Project

### 1. Build the project
```bash
dotnet build
