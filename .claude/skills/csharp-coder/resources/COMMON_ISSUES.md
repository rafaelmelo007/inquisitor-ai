# Common Issues — C# .NET Backend

A reference of build and runtime issues encountered during development, with root causes and fixes.

---

## 1. Missing `using` directives in root Setup.cs

**Symptom:**
```
'IServiceCollection' does not contain a definition for 'AddAuthFeature'
'WebApplication' does not contain a definition for 'MapAuthEndpoints'
```

**Root cause:** The root `Setup.cs` in the Features project calls extension methods defined in feature-specific namespaces (e.g., `InquisitorAI.Features.Auth`, `InquisitorAI.Features.Users`) but is missing the `using` directives for those namespaces.

**Fix:** Add `using` directives for every feature namespace that defines `Add*Feature` or `Map*Endpoints` extension methods:

```csharp
using InquisitorAI.Features.Auth;
using InquisitorAI.Features.InterviewSessions;
using InquisitorAI.Features.Questionnaires;
using InquisitorAI.Features.Scores;
using InquisitorAI.Features.Users;
```

---

## 2. Missing `using` for Infrastructure Setup in Program.cs

**Symptom:**
```
'IServiceCollection' does not contain a definition for 'AddInfrastructure'
```

**Root cause:** `Program.cs` in the Api project calls `services.AddInfrastructure(config)` but is missing the `using` for the namespace where that extension method is defined.

**Fix:** Add to `Program.cs`:

```csharp
using InquisitorAI.Infrastructure.Setup;
```

---

## 3. `System.Speech` assembly not found in .NET 10

**Symptom:**
```
The type or namespace name 'Speech' does not exist in the namespace 'System'
```

**Root cause:** `System.Speech` is not available as a framework assembly reference in .NET 10. It must be installed as a NuGet package.

**Fix:** In the UI `.csproj`, replace the assembly reference with a NuGet package:

```xml
<!-- Remove this -->
<Reference Include="System.Speech" />

<!-- Add this -->
<PackageReference Include="System.Speech" Version="9.0.3" />
```

---

## 4. `AddFixedWindowLimiter` not available in .NET 10

**Symptom:**
```
'RateLimiterOptions' does not contain a definition for 'AddFixedWindowLimiter'
```

**Root cause:** The `AddFixedWindowLimiter` convenience method was removed in .NET 10. The rate limiter must be configured using `GlobalLimiter` with `PartitionedRateLimiter.Create`.

**Fix:** Replace the convenience method with the explicit pattern:

```csharp
using System.Threading.RateLimiting;

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
```

---

## 5. Missing `Xunit` namespace in test files

**Symptom:**
```
The type or namespace name 'Fact' could not be found
The type or namespace name 'FactAttribute' could not be found
```

**Root cause:** Test files are missing `using Xunit;`.

**Fix:** Create a `GlobalUsings.cs` file in the test project root to avoid adding the `using` to every file:

```csharp
// InquisitorAI.Tests/GlobalUsings.cs
global using Xunit;
```

---

## 6. WinForms designer serialization warnings (WFO1000)

**Symptom:**
```
WFO1000: Property 'QuestionnaireId' on type should have DesignerSerializationVisibility attribute
```

**Root cause:** Public properties on WinForms forms that are not designer-managed trigger this warning/error.

**Fix:** Add the attribute to exclude them from designer serialization:

```csharp
using System.ComponentModel;

[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
public long QuestionnaireId { get; set; }
```

---

## 7. EF Core migration: "doesn't reference Microsoft.EntityFrameworkCore.Design"

**Symptom:**
```
Your startup project 'InquisitorAI.Api' doesn't reference Microsoft.EntityFrameworkCore.Design.
```

**Root cause:** The EF Core CLI tools require `Microsoft.EntityFrameworkCore.Design` in the startup project.

**Fix:** Add to the Api `.csproj`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.3">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

---

## 8. EF Core migration: "The entry point exited without ever building an IHost"

**Symptom:**
```
An error occurred while accessing the Microsoft.Extensions.Hosting services.
No DbContext named 'AppDbContext' was found.
```

**Root cause:** `Program.cs` calls `DotNetEnv.Env.Load()` or other startup logic that fails at design time (e.g., missing `.env` values, database not reachable). EF tools cannot build the host, and without it, cannot discover the DbContext.

**Fix:** Add a `IDesignTimeDbContextFactory<AppDbContext>` in the **startup project** (Api):

```csharp
// InquisitorAI.Api/DesignTimeDbContextFactory.cs
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
```

The factory must be in the **startup project** assembly — EF tools look there first.

---

## 9. EF Core migration: "doesn't match your migrations assembly"

**Symptom:**
```
Your target project 'InquisitorAI.Infrastructure' doesn't match your migrations assembly 'InquisitorAI.Features'.
```

**Root cause:** The `AppDbContext` lives in the Features project, so EF defaults to placing migrations there. When using `--project InquisitorAI.Infrastructure`, there's a mismatch.

**Fix:** Configure `MigrationsAssembly` in both the runtime registration and the design-time factory:

```csharp
// InfrastructureSetup.cs
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, b =>
        b.MigrationsAssembly(typeof(InfrastructureSetup).Assembly.FullName)));

// DesignTimeDbContextFactory.cs
optionsBuilder.UseNpgsql(connectionString, b =>
    b.MigrationsAssembly("InquisitorAI.Infrastructure"));
```

Then the migration command works:

```bash
dotnet ef migrations add InitialCreate \
  --project InquisitorAI.Infrastructure \
  --startup-project InquisitorAI.Api \
  --context AppDbContext \
  --output-dir Migrations
```

---

## 10. "ConnectionStrings:Default is not configured" despite `.env` file existing

**Symptom:**
```
System.InvalidOperationException: ConnectionStrings:Default is not configured.
```

**Root cause:** `DotNetEnv.Env.Load()` is called **after** `WebApplication.CreateBuilder(args)`. The builder reads environment variables during construction, so the `.env` values are not yet available when configuration is built.

**Fix:** Move `DotNetEnv.Env.Load()` **before** `CreateBuilder()`:

```csharp
// Wrong — .env loaded too late
var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Correct — .env loaded before builder reads env vars
DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
```

`DotNetEnv.Env.Load()` sets values as process environment variables. `CreateBuilder()` reads them. The order matters — load first, build second.
