using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquisitorAI.Features.InterviewSessions.Domain;

public class InterviewSessionConfiguration : IEntityTypeConfiguration<InterviewSession>
{
    public void Configure(EntityTypeBuilder<InterviewSession> builder)
    {
        builder.ToTable("inq_interview_sessions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(s => s.QuestionnaireId).HasColumnName("questionnaire_id").IsRequired();
        builder.Property(s => s.StartedAt).HasColumnName("started_at").IsRequired();
        builder.Property(s => s.EndedAt).HasColumnName("ended_at");
        builder.Property(s => s.DurationSeconds).HasColumnName("duration_seconds");
        builder.Property(s => s.FinalScore).HasColumnName("final_score");
        builder.Property(s => s.Classification).HasColumnName("classification").HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.ReportContent).HasColumnName("report_content").HasColumnType("text");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(s => s.RowVersion).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();

        builder.HasOne(s => s.User)
            .WithMany(u => u.InterviewSessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Questionnaire)
            .WithMany()
            .HasForeignKey(s => s.QuestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
