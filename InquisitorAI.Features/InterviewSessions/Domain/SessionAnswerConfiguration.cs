using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquisitorAI.Features.InterviewSessions.Domain;

public class SessionAnswerConfiguration : IEntityTypeConfiguration<SessionAnswer>
{
    public void Configure(EntityTypeBuilder<SessionAnswer> builder)
    {
        builder.ToTable("inq_session_answers");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.SessionId).HasColumnName("session_id").IsRequired();
        builder.Property(a => a.QuestionId).HasColumnName("question_id").IsRequired();
        builder.Property(a => a.Transcript).HasColumnName("transcript");
        builder.Property(a => a.Score).HasColumnName("score");
        builder.Property(a => a.AiFeedback).HasColumnName("ai_feedback");
        builder.Property(a => a.Strengths).HasColumnName("strengths");
        builder.Property(a => a.Weaknesses).HasColumnName("weaknesses");
        builder.Property(a => a.ImprovementSuggestions).HasColumnName("improvement_suggestions");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(a => a.RowVersion).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();

        builder.HasOne(a => a.Session)
            .WithMany(s => s.SessionAnswers)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Question)
            .WithMany()
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
