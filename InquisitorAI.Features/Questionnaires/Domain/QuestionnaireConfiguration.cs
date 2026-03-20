using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquisitorAI.Features.Questionnaires.Domain;

public class QuestionnaireConfiguration : IEntityTypeConfiguration<Questionnaire>
{
    public void Configure(EntityTypeBuilder<Questionnaire> builder)
    {
        builder.ToTable("inq_questionnaires");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).HasColumnName("id");
        builder.Property(q => q.Name).HasColumnName("name").IsRequired().HasMaxLength(500);
        builder.Property(q => q.CreatedByUserId).HasColumnName("created_by_user_id").IsRequired();
        builder.Property(q => q.IsPublic).HasColumnName("is_public").IsRequired();
        builder.Property(q => q.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(q => q.RowVersion).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();

        builder.HasOne(q => q.User)
            .WithMany(u => u.Questionnaires)
            .HasForeignKey(q => q.CreatedByUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
