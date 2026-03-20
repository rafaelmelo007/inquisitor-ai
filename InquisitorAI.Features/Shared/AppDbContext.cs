using InquisitorAI.Features.Auth.Domain;
using InquisitorAI.Features.InterviewSessions.Domain;
using InquisitorAI.Features.Questionnaires.Domain;
using InquisitorAI.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace InquisitorAI.Features.Shared;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<SessionAnswer> SessionAnswers => Set<SessionAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
