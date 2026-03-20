---
name: csharp-coder
description: Use this skill whenever the user asks to create, scaffold, or modify a C# / .NET backend application, API, or microservice. Triggers include any mention of '.NET', 'C#', 'Web API', 'ASP.NET', 'Entity Framework', or requests to build REST APIs, microservices, background workers, or backend systems in C#. Also use when the user asks to add features, endpoints, services, repositories, tests, or workers to an existing .NET solution. Covers project structure (Vertical Slices), EF Core code-first, Dapper read repositories, JWT/OAuth authentication, FluentValidation, FluentAssertions, OpenTelemetry + Serilog logging, and background worker hosting. If the user mentions building a 'backend', 'API', or 'service' and the tech stack is .NET or C#, use this skill.
---

# C# .NET Backend Coder

Generate production-ready .NET backend solutions following Vertical Slice Architecture with consistent patterns for APIs, workers, testing, and observability.

## Async Rules

Use `async`/`await` **only** when the method performs I/O against an external resource: database, HTTP call, file system, message queue, cache, or any other network-bound operation. Do **not** make a method async just because it calls another method — async must be justified by actual I/O.

**Use async when:**
- Executing a database query or command (EF Core, Dapper)
- Sending or receiving an HTTP request (`HttpClient`)
- Reading or writing files
- Interacting with a message broker or cache

**Do not use async for:**
- In-memory computation or transformation (mapping, validation logic, calculations)
- Methods that only call other synchronous code
- Extension methods like `.ToDto()`, `.ToDomain()`

Never use `.Result`, `.Wait()`, or `GetAwaiter().GetResult()` — these block threads and risk deadlocks.

```csharp
// Correct — async because it hits the database
public async Task<CustomerDto> HandleAsync(CreateCustomerCommand command, CancellationToken ct = default)
{
    var entity = command.ToDomain();          // sync — in-memory, no async needed
    context.Customers.Add(entity);
    await context.SaveChangesAsync(ct);       // async — I/O
    return entity.ToDto();                    // sync — in-memory, no async needed
}

// Wrong — no I/O, async is unnecessary noise
public async Task<CustomerDto> MapAsync(Customer entity)
{
    return await Task.FromResult(entity.ToDto());
}

// Correct — just sync
public CustomerDto Map(Customer entity) => entity.ToDto();
```

## Solution Structure

Every solution has five projects. Replace `<AppName>` with the actual application name (PascalCase).

```
<AppName>/
├── <AppName>.Features/          # Business logic, domain, endpoints
│   ├── Setup.cs                 # Root register (RegisterAllServices + MapAllEndpoints)
│   ├── Shared/                  # Cross-cutting concerns
│   │   ├── IHandler.cs          # ICommand, IQuery, ICommandHandler, IQueryHandler interfaces
│   │   ├── IDateTimeService.cs  # Abstraction over DateTimeOffset.UtcNow — mockable in tests
│   │   ├── NotificationHandler.cs
│   │   └── AppDbContext.cs      # EF Core DbContext
│   └── <FeatureName>/           # One folder per feature (e.g., Customers, Orders)
│       ├── Setup.cs             # Feature-level DI registration and endpoint mapping
│       ├── Domain/              # EF Core entities + IEntityTypeConfiguration files
│       ├── Dtos/                # Request/response DTOs
│       ├── Commands/            # Command records + handlers (one file per command, EF Core writes)
│       ├── Queries/             # Query records + handlers (one file per query, Dapper reads)
│       ├── Endpoints/           # Minimal API endpoint definitions (one file per endpoint)
│       ├── Extensions/          # Mapping extension methods (e.g., ToDto, ToDomain) — only if needed
│       ├── Configs/             # Feature-specific configuration classes — only if needed
│       └── Workers/             # Background workers — only if this feature has workers
├── <AppName>.Api/               # Host for HTTP endpoints
│   └── Dockerfile               # Multi-stage Dockerfile for the API
├── <AppName>.Worker/            # Host for background workers
│   └── Dockerfile               # Multi-stage Dockerfile for the Worker
├── <AppName>.Migrations/        # EF Core migrations (optional, or inside Features)
├── <AppName>.Tests/             # Unit tests
│   ├── Commands/                # Command handler tests
│   ├── Queries/                 # Query handler tests
│   └── Endpoints/               # Endpoint tests
├── docker-compose.yml           # Orchestrates API, Worker, and dependencies (DB, etc.)
├── .env                         # Environment variables
└── .dockerignore
```

**Only create subfolders that have files.** `Domain/`, `Commands/`, `Queries/`, and `Endpoints/` are present in almost every feature. `Extensions/`, `Configs/`, and `Workers/` are created only when the feature actually needs them — never scaffold empty folders.

### Root Setup.cs (Features project)

```csharp
public static class Setup
{
    public static IServiceCollection RegisterAllServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddCustomerFeature(config);
        services.AddOrderFeature(config);
        // ...one call per feature
        return services;
    }

    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        app.MapCustomerEndpoints();
        app.MapOrderEndpoints();
        // ...one call per feature
        return app;
    }
}
```

### Feature-level Setup.cs

Each feature folder has its own `Setup.cs` that registers only that feature's handlers, validators, configs, and endpoints. The root `Setup.cs` delegates to each feature's setup.

### AppDbContext

Define `AppDbContext` in `Shared/` and apply all entity configurations via `ApplyConfigurationsFromAssembly`:

```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

Register in DI:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("Default")));
```

### IDateTimeService

Never use `DateTime.Now`, `DateTime.UtcNow`, or `DateTimeOffset.UtcNow` directly in handlers or domain code. Always inject `IDateTimeService` so tests can control time without clock dependencies.

Always use `DateTimeOffset` (not `DateTime`) — it preserves timezone information in logs and audit fields.

```csharp
// Shared/IDateTimeService.cs
public interface IDateTimeService
{
    DateTimeOffset UtcNow { get; }
}

public class DateTimeService : IDateTimeService
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
```

Register as singleton (stateless):

```csharp
services.AddSingleton<IDateTimeService, DateTimeService>();
```

In tests, mock it:

```csharp
var mockClock = new Mock<IDateTimeService>();
mockClock.Setup(c => c.UtcNow).Returns(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));
```

## CQRS Pattern (No MediatR)

Use lightweight interfaces and direct DI injection — no dispatcher, no pipeline.

### Core Interfaces

Define these once in the root of the Features project (e.g., `Shared/IHandler.cs`):

```csharp
public interface ICommand { }
public interface ICommand<TResult> { }
public interface IQuery<TResult> { }

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
```

### Command (co-located with handler, EF Core for writes)

Commands that can fail return a nullable result (`TResult?`). The endpoint **always** checks `notifications.HasErrors` before using the result — never access the return value without checking first.

```csharp
// Features/Customers/Commands/CreateCustomer.cs
public record CreateCustomerCommand(string FullName, string Email) : ICommand<CustomerDto?>;

public class CreateCustomerHandler(AppDbContext context, NotificationHandler notifications, IDateTimeService clock)
    : ICommandHandler<CreateCustomerCommand, CustomerDto?>
{
    public async Task<CustomerDto?> HandleAsync(CreateCustomerCommand command, CancellationToken ct = default)
    {
        if (await context.Customers.AnyAsync(c => c.Email == command.Email, ct))
        {
            notifications.AddError("Email already in use.");
            return null;
        }
        var now = clock.UtcNow;
        var entity = new Customer { FullName = command.FullName, Email = command.Email, CreatedAt = now, UpdatedAt = now };
        context.Customers.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity.ToDto();
    }
}
```

### Query (co-located with handler, Dapper for reads)

```csharp
// Features/Customers/Queries/GetCustomerById.cs
public record GetCustomerByIdQuery(long Id) : IQuery<CustomerDto?>;

public class GetCustomerByIdHandler(IDbConnection db)
    : IQueryHandler<GetCustomerByIdQuery, CustomerDto?>
{
    public async Task<CustomerDto?> HandleAsync(GetCustomerByIdQuery query, CancellationToken ct = default)
    {
        const string sql = "SELECT id, full_name, email FROM customers WHERE id = @Id";
        return await db.QuerySingleOrDefaultAsync<CustomerDto>(sql, new { query.Id });
    }
}
```

### Endpoint — inject handler directly

`CancellationToken` is injected automatically by Minimal API — always include it and forward it to `HandleAsync`.

```csharp
app.MapPost("/customers", async (
    CreateCustomerCommand command,
    ICommandHandler<CreateCustomerCommand, CustomerDto?> handler,
    NotificationHandler notifications,
    CancellationToken ct) =>
{
    var result = await handler.HandleAsync(command, ct);
    return notifications.HasErrors
        ? Results.BadRequest(new { errors = notifications.Errors })
        : Results.Created($"/customers/{result!.Id}", result);
});
```

### DI Registration (feature Setup.cs)

Register each handler explicitly — no assembly scanning. Also register `IDbConnection` for query handlers:

```csharp
// IDbConnection — used by all query handlers
services.AddScoped<IDbConnection>(_ => new SqlConnection(config.GetConnectionString("Default")));

// Command handlers
services.AddScoped<ICommandHandler<CreateCustomerCommand, CustomerDto?>, CreateCustomerHandler>();
services.AddScoped<ICommandHandler<UpdateCustomerCommand>, UpdateCustomerHandler>();
services.AddScoped<ICommandHandler<DeleteCustomerCommand>, DeleteCustomerHandler>();

// Query handlers
services.AddScoped<IQueryHandler<GetCustomerByIdQuery, CustomerDto?>, GetCustomerByIdHandler>();
services.AddScoped<IQueryHandler<GetAllCustomersQuery, IEnumerable<CustomerDto>>, GetAllCustomersHandler>();
```

### Rules

- **Commands** mutate state — use EF Core, may return a result, check `NotificationHandler` before proceeding.
- **Queries** are read-only — use Dapper, never mutate state, never depend on `NotificationHandler`.
- Command record + handler live in the **same file** inside `Commands/`.
- Query record + handler live in the **same file** inside `Queries/`.
- Inject handlers directly into endpoints. No dispatcher, no bus.
- Mock handlers with Moq in endpoint tests; test handlers in isolation in command/query tests.

## Domain & Data Access

### Entity Framework (Code-First, Write Operations)

- Target the latest stable .NET and EF Core versions.
- Every entity has a `long` primary key named `Id`.
- Every entity includes audit timestamps and a `RowVersion` for optimistic concurrency:

```csharp
public DateTimeOffset CreatedAt { get; set; }
public DateTimeOffset UpdatedAt { get; set; }

[Timestamp]
public byte[] RowVersion { get; set; }
```

Map in configuration:
```csharp
builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").IsRequired();
builder.Property(c => c.RowVersion).HasColumnName("row_version").IsRowVersion();
```

Set `CreatedAt` and `UpdatedAt` via `IDateTimeService` in the command handler before saving. On update, only set `UpdatedAt`. Never use `DateTime.UtcNow` or `DateTimeOffset.UtcNow` directly — always go through `IDateTimeService`.

- Each entity gets its own `IEntityTypeConfiguration<T>` file inside the feature's `Domain/` folder, alongside the entity class.
- Table and column naming convention: **snake_case** (e.g., `customer_orders`, `created_at`).

```csharp
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(200);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(c => c.RowVersion).HasColumnName("row_version").IsRowVersion();
    }
}
```

### Migrations

Use `dotnet ef` CLI to manage migrations. Name migrations descriptively (e.g., `AddCustomerTable`, `AddOrderStatusColumn`). Migrations live either in a dedicated `<AppName>.Migrations` project or inside the Features project under a top-level `Migrations/` folder. Always review generated migration code before applying. Never use auto-migrate at startup in production — apply migrations explicitly via CI/CD or CLI.

### Dapper (Read-Only, inside Query Handlers)

Dapper SQL lives directly inside query handlers — no separate repository class needed. Query handlers receive `IDbConnection` (not DbContext) and execute raw SQL. All must be async.

EF Core is used exclusively for writes (insert, update, delete). Dapper handles all reads.

## DTO Mapping

All mapping between Domain entities and DTOs is done manually — no AutoMapper or third-party mapping libraries.

```csharp
public static CustomerDto ToDto(this Customer entity) =>
    new(entity.Id, entity.FullName, entity.Email);

// Note: ToDomain does NOT set timestamps — the handler injects IDateTimeService for that.
public static Customer ToDomain(this CreateCustomerCommand command) =>
    new() { FullName = command.FullName, Email = command.Email };
```

## Handler Method Signatures

All handler `HandleAsync` methods follow the same signature: accept the command/query record and a `CancellationToken`, return `Task` or `Task<TResult>`. Always use records for commands and queries — they are immutable and support structural equality.

```csharp
// Command with result (nullable — caller must check NotificationHandler.HasErrors before using)
Task<CustomerDto?> HandleAsync(CreateCustomerCommand command, CancellationToken ct = default);

// Command without result
Task HandleAsync(DeleteCustomerCommand command, CancellationToken ct = default);

// Query (nullable — not found returns null)
Task<CustomerDto?> HandleAsync(GetCustomerByIdQuery query, CancellationToken ct = default);
```

## Error Handling

### NotificationHandler (Expected Errors)

Avoid throwing exceptions whenever possible. Use a `NotificationHandler` to accumulate errors across layers:

```csharp
public class NotificationHandler
{
    private readonly List<string> _errors = [];
    public IReadOnlyList<string> Errors => _errors;
    public bool HasErrors => _errors.Count > 0;
    public void AddError(string message) => _errors.Add(message);
}
```

- Register `NotificationHandler` as **scoped** (one instance per request).
- Every command handler checks `NotificationHandler.HasErrors` before proceeding — if errors exist from a prior step, return early.
- Endpoints check `NotificationHandler.HasErrors` after calling the handler and return `Results.BadRequest` if any errors were collected. Never access the handler's return value without checking first.

### Global Exception Middleware (Unexpected Errors)

For truly unexpected exceptions (database connection failures, serialization bugs, unhandled edge cases), add a global exception-handling middleware. This catches anything not handled by `NotificationHandler` and returns a consistent 500 response with a correlation ID for tracing via OpenTelemetry.

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var correlationId = Activity.Current?.Id ?? context.TraceIdentifier;
        Log.Error("Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred.",
            correlationId
        });
    });
});
```

## Endpoints & Validation

### Minimal API Endpoints

Each endpoint lives in its own file inside the feature's `Endpoints/` folder, named after the operation (e.g., `CreateCustomer.cs`, `GetCustomerById.cs`, `UpdateCustomer.cs`, `DeleteCustomer.cs`). This keeps each file small, focused, and easy to locate. Expose all endpoints via Swagger (Swashbuckle or built-in OpenAPI).

### FluentValidation

Every command record that enters an endpoint has a corresponding `AbstractValidator<T>`. Validate in the endpoint before calling the handler — push failures into `NotificationHandler` and return early.

```csharp
// Features/Customers/Commands/CreateCustomer.cs — validator alongside the command
public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

// Endpoint
app.MapPost("/customers", async (
    CreateCustomerCommand command,
    IValidator<CreateCustomerCommand> validator,
    ICommandHandler<CreateCustomerCommand, CustomerDto?> handler,
    NotificationHandler notifications,
    CancellationToken ct) =>
{
    var validation = validator.Validate(command);
    if (!validation.IsValid)
    {
        validation.Errors.ForEach(e => notifications.AddError(e.ErrorMessage));
        return Results.BadRequest(new { errors = notifications.Errors });
    }

    var result = await handler.HandleAsync(command, ct);
    return notifications.HasErrors
        ? Results.BadRequest(new { errors = notifications.Errors })
        : Results.Created($"/customers/{result!.Id}", result);
});
```

Register validators in the feature's `Setup.cs`:

```csharp
services.AddValidatorsFromAssemblyContaining<CreateCustomerValidator>();
```

## Authentication & Authorization

### JWT + OAuth

- Support OAuth via Google and GitHub as the preferred sign-in method.
- Also support custom email/password authentication with asymmetric hashing (one-way hash + salt, using bcrypt or Argon2).
- Store custom users in a `users` table (snake_case, like all other tables).
- JWT tokens signed with a configurable security key (stored in `.env`).
- All endpoints require authentication **except**: sign-in, sign-up, sign-out, and reset-password.

## Configuration & Environment

### DotEnv Support

All projects load a `.env` file at startup, which overrides `appsettings.json` values. Use the `DotNetEnv` library.

```csharp
DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();
```

### Worker Configuration

Each worker's iteration interval is configurable via settings:

```json
{
  "Workers": {
    "OrderSyncWorker": { "IntervalSeconds": 60 },
    "EmailDispatchWorker": { "IntervalSeconds": 30 }
  }
}
```

## CORS & Rate Limiting

Configure CORS for frontend SPA consumption. Define allowed origins in `.env` or `appsettings.json`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

Add rate limiting middleware for public-facing endpoints using `Microsoft.AspNetCore.RateLimiting` with configurable limits per endpoint group.

## Health Checks

Register health check endpoints for readiness and liveness probes — essential for microservice deployments:

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Name == "self" });
```

Health check endpoints are excluded from authentication.

## Background Workers

The `<AppName>.Worker` project hosts all workers defined in feature folders. Workers inherit from `BackgroundService`, read their interval from configuration, and use the same handler and notification infrastructure as the API. All worker logic must be async.

Workers create a DI scope per iteration and resolve command handlers by interface — never by concrete class.

```csharp
public class OrderSyncWorker(IServiceScopeFactory scopeFactory, IConfiguration config) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = config.GetValue<int>("Workers:OrderSyncWorker:IntervalSeconds", 60);
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider
                .GetRequiredService<ICommandHandler<SyncOrdersCommand>>();
            await handler.HandleAsync(new SyncOrdersCommand(), stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }
}
```

## Logging & Observability

### Serilog + OpenTelemetry

- Use Serilog as the logging provider, writing to files at `/logs/<app_name>/`.
- Integrate OpenTelemetry for traces and metrics.
- For built-in framework events (Microsoft.*, System.*), log only `Warning` and `Error`. Application-level events use `Information` and above.

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.File($"/logs/{appName}/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

Ensure consistent, structured logging across all handlers, endpoints, and workers — every significant operation should log its start, completion, and any errors.

## Testing

### Conventions

- Use **xUnit** as the test framework.
- Use **FluentAssertions** for all assertions.
- Use **Moq** for mocking all interfaces and dependencies.
- Use **Bogus** to generate fake test data whenever possible — avoid hardcoded test values.
- Every test follows **Arrange → Act → Assert**.
- Every command handler, query handler, and endpoint gets at least one test.
- Use `async Task` only when the test actually awaits something — same rule as application code.

Command handler tests use EF Core's `UseInMemoryDatabase` — never mock `AppDbContext` directly (it's a concrete class and Moq is unreliable against it). Endpoint tests mock the handler interface.

```csharp
// Tests/Commands/CreateCustomerHandlerTests.cs
public class CreateCustomerHandlerTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IDateTimeService> _mockClock = new();
    private readonly DateTimeOffset _fixedNow = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public CreateCustomerHandlerTests()
    {
        _mockClock.Setup(c => c.UtcNow).Returns(_fixedNow);
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ReturnsCustomerDto()
    {
        // Arrange
        await using var context = CreateContext();
        var command = new CreateCustomerCommand(_faker.Person.FullName, _faker.Internet.Email());
        var notifications = new NotificationHandler();
        var handler = new CreateCustomerHandler(context, notifications, _mockClock.Object);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeFalse();
        result.Should().NotBeNull();
        result!.FullName.Should().Be(command.FullName);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateEmail_AddsError()
    {
        // Arrange
        await using var context = CreateContext();
        var email = _faker.Internet.Email();
        context.Customers.Add(new Customer { FullName = "Existing", Email = email, CreatedAt = _fixedNow, UpdatedAt = _fixedNow });
        await context.SaveChangesAsync();

        var command = new CreateCustomerCommand(_faker.Person.FullName, email);
        var notifications = new NotificationHandler();
        var handler = new CreateCustomerHandler(context, notifications, _mockClock.Object);

        // Act
        await handler.HandleAsync(command);

        // Assert
        notifications.HasErrors.Should().BeTrue();
        notifications.Errors.Should().ContainSingle().Which.Should().Contain("email");
    }
}

// Tests/Endpoints/CreateCustomerEndpointTests.cs — mock the handler interface
public class CreateCustomerEndpointTests
{
    private readonly Mock<ICommandHandler<CreateCustomerCommand, CustomerDto?>> _mockHandler = new();
    private readonly Faker _faker = new();

    [Fact]
    public async Task Post_WithValidCommand_Returns201()
    {
        // Arrange
        var command = new CreateCustomerCommand(_faker.Person.FullName, _faker.Internet.Email());
        var expected = new CustomerDto(1, command.FullName, command.Email);
        _mockHandler.Setup(h => h.HandleAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(expected);
        var notifications = new NotificationHandler();

        // Act — call endpoint delegate directly
        var result = await CreateCustomerEndpoint.Handle(command, _mockHandler.Object, notifications, default);

        // Assert
        result.Should().BeOfType<Created<CustomerDto>>();
    }
}
```

Add `Microsoft.EntityFrameworkCore.InMemory` to the test project.

### Bogus Usage Patterns

Use `Faker` for simple random values and `Faker<T>` with rules for generating complete domain objects or DTOs:

```csharp
// Simple values via Faker
var faker = new Faker();
var name = faker.Person.FullName;
var email = faker.Internet.Email();
var id = faker.Random.Long(1, 10000);

// Typed fakers for domain objects
var customerFaker = new Faker<Customer>()
    .RuleFor(c => c.Id, f => f.Random.Long(1, 10000))
    .RuleFor(c => c.FullName, f => f.Person.FullName)
    .RuleFor(c => c.Email, f => f.Internet.Email())
    .RuleFor(c => c.CreatedAt, f => new DateTimeOffset(f.Date.Recent(), TimeSpan.Zero))
    .RuleFor(c => c.UpdatedAt, f => new DateTimeOffset(f.Date.Recent(), TimeSpan.Zero));

var fakeCustomer = customerFaker.Generate();
var fakeCustomers = customerFaker.Generate(10);
```

## Docker

Both `<AppName>.Api` and `<AppName>.Worker` include their own multi-stage Dockerfile. Use the same pattern for both — only the entry project changes.

```dockerfile
# <AppName>.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["<AppName>.Api/<AppName>.Api.csproj", "<AppName>.Api/"]
COPY ["<AppName>.Features/<AppName>.Features.csproj", "<AppName>.Features/"]
RUN dotnet restore "<AppName>.Api/<AppName>.Api.csproj"
COPY . .
RUN dotnet publish "<AppName>.Api/<AppName>.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "<AppName>.Api.dll"]
```

For the Worker, replace `.Api` with `.Worker` and remove the `EXPOSE` line.

### docker-compose.yml

Orchestrate the API, Worker, database, and any other dependencies:

```yaml
services:
  api:
    build:
      context: .
      dockerfile: <AppName>.Api/Dockerfile
    ports:
      - "8080:8080"
    env_file: .env
    depends_on:
      db:
        condition: service_healthy
    volumes:
      - logs:/logs

  worker:
    build:
      context: .
      dockerfile: <AppName>.Worker/Dockerfile
    env_file: .env
    depends_on:
      db:
        condition: service_healthy
    volumes:
      - logs:/logs

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${DB_PASSWORD}
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    healthcheck:
      test: ["CMD", "/opt/mssql-tools/bin/sqlcmd", "-S", "localhost", "-U", "sa", "-P", "${DB_PASSWORD}", "-Q", "SELECT 1"]
      interval: 10s
      timeout: 5s
      retries: 10
      start_period: 30s

volumes:
  sqldata:
  logs:
```

Include a `.dockerignore` at the solution root to exclude `bin/`, `obj/`, `.env`, and test projects from the build context.

## NuGet Packages (latest stable versions)

Always target the latest stable .NET and package versions:

| Purpose | Package |
|---|---|
| ORM (writes) | Microsoft.EntityFrameworkCore + provider |
| Reads | Dapper |
| Validation | FluentValidation |
| Test assertions | FluentAssertions |
| Test framework | xUnit |
| Mocking | Moq |
| Fake data | Bogus |
| EF Core testing | Microsoft.EntityFrameworkCore.InMemory |
| Logging | Serilog, Serilog.Sinks.File |
| Observability | OpenTelemetry SDK + exporters |
| Auth | Microsoft.AspNetCore.Authentication.JwtBearer |
| OAuth | Microsoft.AspNetCore.Authentication.Google, AspNet.Security.OAuth.GitHub |
| Environment | DotNetEnv |
| API docs | Swashbuckle.AspNetCore or built-in OpenAPI |
| Health checks | Microsoft.Extensions.Diagnostics.HealthChecks |
| Rate limiting | Microsoft.AspNetCore.RateLimiting |
