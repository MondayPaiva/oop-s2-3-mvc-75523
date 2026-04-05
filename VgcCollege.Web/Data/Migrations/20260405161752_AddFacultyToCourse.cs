using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VgcCollege.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFacultyToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacultyProfileId",
                table: "Courses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_FacultyProfileId",
                table: "Courses",
                column: "FacultyProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_FacultyProfiles_FacultyProfileId",
                table: "Courses",
                column: "FacultyProfileId",
                principalTable: "FacultyProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_FacultyProfiles_FacultyProfileId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_FacultyProfileId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "FacultyProfileId",
                table: "Courses");
        }
    }
}
