using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquisitorAI.Features.Questionnaires.Domain;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("inq_questions");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).HasColumnName("id");
        builder.Property(q => q.QuestionnaireId).HasColumnName("questionnaire_id").IsRequired();
        builder.Property(q => q.OrderIndex).HasColumnName("order_index").IsRequired();
        builder.Property(q => q.Category).HasColumnName("category").HasMaxLength(200);
        builder.Property(q => q.Difficulty).HasColumnName("difficulty").HasConversion<string>().HasMaxLength(50);
        builder.Property(q => q.QuestionText).HasColumnName("question_text").IsRequired();
        builder.Property(q => q.IdealAnswer).HasColumnName("ideal_answer").IsRequired();
        builder.Property(q => q.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(q => q.RowVersion).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();

        builder.HasOne(q => q.Questionnaire)
            .WithMany(qn => qn.Questions)
            .HasForeignKey(q => q.QuestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
