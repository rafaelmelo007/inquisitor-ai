using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InquisitorAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UseXminForConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "row_version", table: "inq_users");
            migrationBuilder.DropColumn(name: "row_version", table: "inq_questionnaires");
            migrationBuilder.DropColumn(name: "row_version", table: "inq_questions");
            migrationBuilder.DropColumn(name: "row_version", table: "inq_refresh_tokens");
            migrationBuilder.DropColumn(name: "row_version", table: "inq_interview_sessions");
            migrationBuilder.DropColumn(name: "row_version", table: "inq_session_answers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(name: "row_version", table: "inq_users", type: "bytea", rowVersion: true, nullable: false);
            migrationBuilder.AddColumn<byte[]>(name: "row_version", table: "inq_questionnaires", type: "bytea", rowVersion: true, nullable: false);
            migrationBuilder.AddColumn<byte[]>(name: "row_version", table: "inq_questions", type: "bytea", rowVersion: true, nullable: false);
            migrationBuilder.AddColumn<byte[]>(name: "row_version", table: "inq_refresh_tokens", type: "bytea", rowVersion: true, nullable: false);
            migrationBuilder.AddColumn<byte[]>(name: "row_version", table: "inq_interview_sessions", type: "bytea", rowVersion: true, nullable: false);
            migrationBuilder.AddColumn<byte[]>(name: "row_version", table: "inq_session_answers", type: "bytea", rowVersion: true, nullable: false);
        }
    }
}
