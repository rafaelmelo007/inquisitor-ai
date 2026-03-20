using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InquisitorAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "inq_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    external_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inq_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inq_questionnaires",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inq_questionnaires", x => x.id);
                    table.ForeignKey(
                        name: "FK_inq_questionnaires_inq_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "inq_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inq_refresh_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inq_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_inq_refresh_tokens_inq_users_user_id",
                        column: x => x.user_id,
                        principalTable: "inq_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inq_interview_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    questionnaire_id = table.Column<long>(type: "bigint", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    final_score = table.Column<decimal>(type: "numeric", nullable: true),
                    classification = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    report_content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inq_interview_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_inq_interview_sessions_inq_questionnaires_questionnaire_id",
                        column: x => x.questionnaire_id,
                        principalTable: "inq_questionnaires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inq_interview_sessions_inq_users_user_id",
                        column: x => x.user_id,
                        principalTable: "inq_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inq_questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    questionnaire_id = table.Column<long>(type: "bigint", nullable: false),
                    order_index = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    difficulty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    ideal_answer = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inq_questions", x => x.id);
                    table.ForeignKey(
                        name: "FK_inq_questions_inq_questionnaires_questionnaire_id",
                        column: x => x.questionnaire_id,
                        principalTable: "inq_questionnaires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inq_session_answers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    session_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    transcript = table.Column<string>(type: "text", nullable: true),
                    score = table.Column<decimal>(type: "numeric", nullable: true),
                    ai_feedback = table.Column<string>(type: "text", nullable: true),
                    strengths = table.Column<string>(type: "text", nullable: true),
                    weaknesses = table.Column<string>(type: "text", nullable: true),
                    improvement_suggestions = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inq_session_answers", x => x.id);
                    table.ForeignKey(
                        name: "FK_inq_session_answers_inq_interview_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "inq_interview_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_inq_session_answers_inq_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "inq_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inq_interview_sessions_questionnaire_id",
                table: "inq_interview_sessions",
                column: "questionnaire_id");

            migrationBuilder.CreateIndex(
                name: "IX_inq_interview_sessions_user_id",
                table: "inq_interview_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_inq_questionnaires_created_by_user_id",
                table: "inq_questionnaires",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_inq_questions_questionnaire_id",
                table: "inq_questions",
                column: "questionnaire_id");

            migrationBuilder.CreateIndex(
                name: "ix_inq_refresh_tokens_token_hash",
                table: "inq_refresh_tokens",
                column: "token_hash");

            migrationBuilder.CreateIndex(
                name: "IX_inq_refresh_tokens_user_id",
                table: "inq_refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_inq_session_answers_question_id",
                table: "inq_session_answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_inq_session_answers_session_id",
                table: "inq_session_answers",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_inq_users_email",
                table: "inq_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inq_users_provider_external_id",
                table: "inq_users",
                columns: new[] { "provider", "external_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inq_refresh_tokens");

            migrationBuilder.DropTable(
                name: "inq_session_answers");

            migrationBuilder.DropTable(
                name: "inq_interview_sessions");

            migrationBuilder.DropTable(
                name: "inq_questions");

            migrationBuilder.DropTable(
                name: "inq_questionnaires");

            migrationBuilder.DropTable(
                name: "inq_users");
        }
    }
}
