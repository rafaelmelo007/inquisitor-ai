using InquisitorAI.Features.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InquisitorAI.Api;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=inquisitor_ai;Username=inquisitor;Password=CHANGE_ME";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString, b =>
            b.MigrationsAssembly("InquisitorAI.Infrastructure"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
