using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InquisitorAI.Features.Users.Domain;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("inq_users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.Provider).HasColumnName("provider").IsRequired()
            .HasConversion<string>().HasMaxLength(50);
        builder.Property(u => u.ExternalId).HasColumnName("external_id").IsRequired().HasMaxLength(256);
        builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(256);
        builder.Property(u => u.DisplayName).HasColumnName("display_name").IsRequired().HasMaxLength(200);
        builder.Property(u => u.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(1024);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(u => u.RowVersion).HasColumnName("xmin").HasColumnType("xid").IsRowVersion();

        builder.HasIndex(u => new { u.Provider, u.ExternalId }).IsUnique()
            .HasDatabaseName("ix_inq_users_provider_external_id");
        builder.HasIndex(u => u.Email).IsUnique()
            .HasDatabaseName("ix_inq_users_email");
    }
}
