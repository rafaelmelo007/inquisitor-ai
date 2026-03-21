using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InquisitorAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageToInterviewSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "inq_interview_sessions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "inq_interview_sessions");
        }
    }
}
