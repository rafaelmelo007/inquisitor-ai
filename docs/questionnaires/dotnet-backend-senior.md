# .NET Backend Senior Developer

## Q1
**Category:** C# Language
**Difficulty:** Medium
**Question:** What is the difference between `ValueTask<T>` and `Task<T>`, and when should you prefer one over the other?
**Ideal Answer:** `Task<T>` always allocates a heap object. `ValueTask<T>` is a struct that avoids allocation when the result is already available synchronously (e.g., cached values, hot paths). Prefer `ValueTask<T>` for high-throughput APIs where the operation frequently completes synchronously. Avoid it when the result is always async, as repeated awaiting or storing it is unsafe and can lead to undefined behavior. `Task<T>` is safer for general use; `ValueTask<T>` is an optimization.

## Q2
**Category:** C# Language
**Difficulty:** Hard
**Question:** Explain how `IAsyncEnumerable<T>` works and describe a real use case where it is preferable to returning a `List<T>` or `IEnumerable<T>`.
**Ideal Answer:** `IAsyncEnumerable<T>` enables asynchronous iteration using `await foreach`. Items are yielded one at a time as they become available, without buffering the entire collection. It is ideal for streaming large result sets from a database (e.g., reading thousands of rows with Dapper's `QueryUnbufferedAsync`), streaming API responses, or reading from message queues. Returning a `List<T>` materializes everything in memory first; `IAsyncEnumerable<T>` reduces memory pressure and time-to-first-result.

## Q3
**Category:** C# Language
**Difficulty:** Medium
**Question:** What are primary constructors in C# 12, and what are the tradeoffs of using them in classes vs. records?
**Ideal Answer:** Primary constructors allow parameters to be declared directly on the class or struct declaration. In records they have been available since C# 9 and generate public init-only properties automatically. In classes (C# 12), the parameters are in scope throughout the class body but are not automatically promoted to properties — they are captured as private fields if referenced. This means they can be mutated unless care is taken. Tradeoff: they reduce boilerplate but can be less explicit about whether a parameter becomes a property or a field, which may hurt readability.

## Q4
**Category:** C# Language
**Difficulty:** Hard
**Question:** What is the difference between `Span<T>`, `Memory<T>`, and `ArraySegment<T>`? When is each appropriate?
**Ideal Answer:** `Span<T>` is a stack-only ref struct providing a window over contiguous memory (array, stack-allocated buffer, or native memory) with no heap allocation. It cannot be stored in fields or used across async boundaries. `Memory<T>` is the heap-compatible counterpart of `Span<T>`, safe to store in fields and use with async code. `ArraySegment<T>` is the older, less efficient predecessor — it only works with arrays and has more overhead. Use `Span<T>` for synchronous, stack-confined, high-performance parsing; use `Memory<T>` for async scenarios; avoid `ArraySegment<T>` in new code.

## Q5
**Category:** C# Language
**Difficulty:** Medium
**Question:** Explain the `record` type in C#. What does the compiler generate, and what is structural vs. referential equality?
**Ideal Answer:** A `record` is a reference type (or `record struct` for value type) where the compiler generates: value-based `Equals` and `GetHashCode` comparing all properties, a `ToString` that prints property values, a copy constructor, and a deconstruct method. Structural equality means two instances with identical property values are considered equal, unlike classes which use referential equality (same memory address). Records also support non-destructive mutation via `with` expressions. They are ideal for DTOs and immutable domain models.

## Q6
**Category:** C# Language
**Difficulty:** Hard
**Question:** What are source generators, and how do they differ from Roslyn analyzers and T4 templates?
**Ideal Answer:** Source generators run during compilation and emit additional C# source files into the compilation. Unlike T4 templates (which run at design time outside the compiler pipeline), source generators have full access to the semantic model via Roslyn APIs and can inspect the code being compiled. Unlike analyzers (which only report diagnostics), generators produce new code. Use cases include eliminating reflection-based serialization (e.g., `System.Text.Json` source gen), generating mapping code, or producing strongly-typed config accessors. They improve startup performance and AOT compatibility.

## Q7
**Category:** C# Language
**Difficulty:** Medium
**Question:** What is the difference between covariance and contravariance in generics? Give a practical example.
**Ideal Answer:** Covariance (`out T`) allows a more derived type to be used where a base type is expected, valid for producers (e.g., `IEnumerable<string>` can be assigned to `IEnumerable<object>`). Contravariance (`in T`) allows a more general type where a derived is expected, valid for consumers (e.g., `Action<object>` can be assigned to `Action<string>`). Only interfaces and delegates support variance. A practical example: `IEnumerable<T>` is covariant, so `IEnumerable<Dog>` is assignable to `IEnumerable<Animal>` — safe because you only read from it and never write.

## Q8
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** What is the difference between `AddScoped`, `AddTransient`, and `AddSingleton` in ASP.NET Core DI, and what problems can arise from misusing them?
**Ideal Answer:** `Singleton` creates one instance for the application lifetime. `Scoped` creates one per HTTP request. `Transient` creates a new instance every time it is requested. A common pitfall is "captive dependencies": injecting a scoped or transient service into a singleton means the short-lived service is effectively promoted to singleton lifetime, leading to stale state or race conditions. EF Core's `DbContext` is scoped and must never be injected into a singleton. ASP.NET Core will throw at startup if this is detected with scope validation enabled.

## Q9
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** How does middleware differ from filters in ASP.NET Core, and when would you use each?
**Ideal Answer:** Middleware operates at the HTTP pipeline level and runs for every request regardless of routing. It handles cross-cutting concerns like logging, CORS, authentication, and exception handling. Filters operate within the MVC/minimal API action pipeline and have access to action context, result, and model state — they run only for requests that reach an endpoint. Use middleware for concerns that apply globally across all requests. Use filters for concerns tied to controller actions (e.g., authorization on specific endpoints, action-level logging, response transformation).

## Q10
**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** Explain how minimal APIs compare to controller-based APIs in ASP.NET Core. What are the performance implications?
**Ideal Answer:** Minimal APIs have a lower overhead than controller-based APIs because they skip MVC's full pipeline: no model binding via reflection on controllers, no action filter pipeline unless explicitly added, and a simpler routing model. They result in faster cold start times and better AOT compatibility. Controller-based APIs provide more built-in conventions (attribute routing, action filters, `[ApiController]` behavior). For high-throughput microservices, minimal APIs are preferred. For large, team-maintained APIs with complex authorization and validation policies, controllers with filters may be more maintainable.

## Q11
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** What is `IOptions<T>`, `IOptionsSnapshot<T>`, and `IOptionsMonitor<T>`? When would you use each?
**Ideal Answer:** `IOptions<T>` is a singleton — configuration is read once at startup and never changes. `IOptionsSnapshot<T>` is scoped — it re-reads configuration per request, useful in web apps where config can change. `IOptionsMonitor<T>` is a singleton that supports change notifications and `OnChange` callbacks, ideal for background services that need to react to config changes at runtime. Use `IOptions<T>` for static config, `IOptionsSnapshot<T>` when per-request freshness is needed, and `IOptionsMonitor<T>` for long-running services.

## Q12
**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** How does the ASP.NET Core request pipeline handle exceptions? Compare `UseExceptionHandler`, `app.UseStatusCodePages`, and a custom middleware approach.
**Ideal Answer:** `UseExceptionHandler` catches unhandled exceptions thrown anywhere down the pipeline and re-executes on the error path (e.g., `/error`). It is the standard production approach. `UseStatusCodePages` adds response bodies for status codes without bodies (e.g., 404, 405) — it does not catch exceptions. A custom middleware wrapping `next()` in a try/catch gives the most control: you can produce a consistent ProblemDetails response, log with a correlation ID, and avoid the re-execution overhead of `UseExceptionHandler`. The tradeoff of custom middleware is increased maintenance responsibility.

## Q13
**Category:** ASP.NET Core
**Difficulty:** Medium
**Question:** What is the role of `IHostedService` and `BackgroundService`? How would you implement a reliable background worker?
**Ideal Answer:** `IHostedService` defines `StartAsync` and `StopAsync` lifecycle hooks. `BackgroundService` is a base class that simplifies this by exposing `ExecuteAsync`. For a reliable worker: override `ExecuteAsync`, run a loop with a `CancellationToken`, use `Task.Delay` or a `PeriodicTimer` for intervals, catch all exceptions to prevent the host from shutting down, and implement graceful shutdown by respecting cancellation. For durable work (e.g., processing queues), prefer using a message broker with at-least-once delivery rather than in-memory queues.

## Q14
**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** What is the N+1 query problem in EF Core, and how do you solve it?
**Ideal Answer:** N+1 occurs when loading a list of N entities and then lazily loading a related entity for each one, resulting in N+1 database round-trips. EF Core solutions: (1) Eager loading with `.Include()` / `.ThenInclude()` — fetches related data in the same query using JOINs. (2) Explicit loading — controlled loading after the fact. (3) Split queries with `.AsSplitQuery()` — runs separate queries per navigation but avoids cartesian explosion on collections. Avoid lazy loading in production APIs as it silently generates excessive queries.

## Q15
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** What is the difference between `SaveChanges` tracking and `AsNoTracking`? When would you use each?
**Ideal Answer:** By default, EF Core tracks entities returned from queries in the `ChangeTracker`. On `SaveChanges`, it detects changes and generates UPDATE statements. `AsNoTracking()` skips tracking — entities are not registered in the change tracker, making reads faster (less memory, no snapshot comparison). Use tracking when you intend to modify and save the entity. Use `AsNoTracking` for read-only queries (e.g., GET endpoints, Dapper-like read models) to improve performance. `AsNoTrackingWithIdentityResolution` is a middle ground: no tracking but still resolves object identity.

## Q16
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** How does EF Core handle optimistic concurrency? What is the `xmin` system column approach in PostgreSQL?
**Ideal Answer:** Optimistic concurrency prevents lost updates by checking that a row has not changed since it was read. EF Core marks a property as a concurrency token with `[ConcurrencyCheck]` or `.IsRowVersion()`. On UPDATE/DELETE, EF adds a WHERE clause on that token; if no rows are affected, it throws `DbUpdateConcurrencyException`. In SQL Server, `rowversion` is auto-generated. In PostgreSQL, the `[Timestamp] byte[]` approach does not work because `bytea` is not auto-generated. The correct PostgreSQL approach uses the `xmin` system column (type `xid`, mapped to `uint` in C#) via `.HasColumnName("xmin").HasColumnType("xid").IsRowVersion()` — Postgres automatically updates `xmin` on every write.

## Q17
**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** What is the difference between code-first and database-first in EF Core? What are the pros and cons of each?
**Ideal Answer:** Code-first: you define the model in C#, EF generates migrations and the schema. Pros: full source control over schema, refactoring-friendly, migrations as code. Cons: complex schemas (e.g., stored procs, advanced indexing) require manual migration edits. Database-first: you scaffold the model from an existing database using `dotnet ef dbcontext scaffold`. Pros: useful when joining an existing DB. Cons: the generated code is verbose, hard to customize, and drifts from the DB as both evolve. For greenfield projects, code-first is strongly preferred.

## Q18
**Category:** Entity Framework Core
**Difficulty:** Hard
**Question:** How would you implement a soft delete pattern in EF Core without modifying every query in the codebase?
**Ideal Answer:** Use a global query filter: define an `IsDeleted` column on the entity and configure `.HasQueryFilter(e => !e.IsDeleted)` in the entity configuration. EF Core automatically appends `WHERE is_deleted = false` to all queries for that entity. To permanently delete, use `.IgnoreQueryFilters()`. Combine with a `SaveChanges` override that intercepts `EntityState.Deleted`, sets `IsDeleted = true` and `DeletedAt = now`, and changes the state to `Modified`. This makes soft delete transparent across the entire codebase.

## Q19
**Category:** Architecture
**Difficulty:** Hard
**Question:** What is the difference between CQRS and a traditional layered architecture? What problems does CQRS solve?
**Ideal Answer:** In a layered architecture, the same model serves reads and writes, often leading to complex aggregates and over-fetching. CQRS (Command Query Responsibility Segregation) separates the write model (commands that change state) from the read model (queries that return data). Benefits: read models can be optimized independently (e.g., denormalized views, Dapper instead of EF), commands can enforce invariants on a rich domain model, and each side can scale independently. It also pairs naturally with Event Sourcing. Tradeoff: more code, eventual consistency in distributed scenarios.

## Q20
**Category:** Architecture
**Difficulty:** Hard
**Question:** What is the Outbox Pattern, and how does it solve the dual-write problem in distributed systems?
**Ideal Answer:** The dual-write problem: saving to a database and publishing an event to a message broker in two separate operations can leave them inconsistent if one fails. The Outbox Pattern solves this by writing both the domain change and the event to the same database transaction (the event goes to an `outbox` table). A separate background process polls the outbox and publishes events to the broker, then marks them as published. This guarantees at-least-once delivery with no lost events. Tools like MassTransit (with EF Core outbox) or Debezium (CDC-based) implement this pattern.

## Q21
**Category:** Architecture
**Difficulty:** Medium
**Question:** Explain the Repository pattern. When does it add value, and when does it add unnecessary abstraction?
**Ideal Answer:** The Repository pattern abstracts data access behind an interface, enabling testability (mock the repository) and decoupling the domain from the data layer. It adds value when: the persistence technology is likely to change, complex query logic needs to be centralized, or unit testing without a real database is required. It adds unnecessary abstraction when: EF Core's `DbContext` (already a unit of work + repository) is wrapped in a thin pass-through repository, adding boilerplate with no benefit. In CQRS, command handlers often use `DbContext` directly, while read models use lightweight query handlers (e.g., Dapper), making a generic repository redundant.

## Q22
**Category:** Architecture
**Difficulty:** Hard
**Question:** What are the trade-offs between a microservices architecture and a modular monolith?
**Ideal Answer:** Microservices: independent deployability, technology heterogeneity, fault isolation, horizontal scaling per service. Costs: network latency, distributed transactions, service discovery, operational complexity (Kubernetes, service mesh, distributed tracing). Modular monolith: same deployment boundary but with enforced internal module boundaries (separate assemblies, no direct cross-module DB access). Easier to develop, test, and deploy. Lower operational overhead. Recommended starting point for most teams — you can extract services when a boundary is proven and the scaling need is real. Premature microservices decomposition is one of the most common architectural mistakes.

## Q23
**Category:** Architecture
**Difficulty:** Medium
**Question:** What is the Vertical Slice Architecture, and how does it differ from a traditional layered (N-tier) architecture?
**Ideal Answer:** In N-tier (Controllers → Services → Repositories → DB), every feature touches all layers horizontally. Vertical Slice organizes code by feature: each feature owns its command/query, handler, and data access in one folder. Benefits: lower coupling between features, changes are isolated to a single slice, easier to onboard (find everything for a feature in one place), and each slice can choose its own pattern (some may use EF, others raw SQL). The tradeoff is potential code duplication across slices and the need for discipline to avoid shared state creeping in.

## Q24
**Category:** Architecture
**Difficulty:** Hard
**Question:** How would you design an idempotent API endpoint? Why is idempotency important, and how is it typically implemented?
**Ideal Answer:** An idempotent operation produces the same result regardless of how many times it is called. GET, PUT, and DELETE are naturally idempotent; POST is not. To make a POST idempotent: require clients to send an `Idempotency-Key` header (a UUID). The server stores the key and the response in a cache or DB. On a duplicate request with the same key, return the stored response without re-executing. This is critical for financial transactions, order creation, and any operation with side effects — it allows safe retries after network failures without duplicating state.

## Q25
**Category:** Architecture
**Difficulty:** Medium
**Question:** What is Domain-Driven Design (DDD)? Explain the concepts of Aggregate, Entity, Value Object, and Bounded Context.
**Ideal Answer:** DDD is an approach to software design that aligns code structure with the business domain. Entity: has a unique identity (e.g., `Order` identified by `OrderId`). Value Object: defined by its attributes, no identity, immutable (e.g., `Money`, `Address`). Aggregate: a cluster of entities and value objects with a root entity that enforces invariants — all access goes through the root. Bounded Context: an explicit boundary within which a domain model is consistent and a ubiquitous language applies. Different contexts may model the same concept differently (e.g., "Customer" in Billing vs. Shipping). Bounded Contexts communicate via well-defined interfaces (events, APIs).

## Q26
**Category:** Performance
**Difficulty:** Hard
**Question:** How would you diagnose and fix a slow API endpoint in a .NET application in production?
**Ideal Answer:** Start with observability: check distributed traces (e.g., OpenTelemetry + Jaeger) to identify where time is spent. Look at database query duration — N+1 queries and missing indexes are the most common causes. Use `EXPLAIN ANALYZE` on slow queries. Check memory allocations with dotMemory or `dotnet-counters` (GC pressure can cause latency spikes). Profile CPU with `dotnet-trace` and analyze with PerfView or SpeedScope. In code, look for synchronous I/O blocking async threads, excessive serialization, or large object allocations. Fix in order: queries first (indexes, projections), then allocations, then algorithmic complexity.

## Q27
**Category:** Performance
**Difficulty:** Medium
**Question:** What is the difference between `IMemoryCache` and `IDistributedCache` in ASP.NET Core? When would you use each?
**Ideal Answer:** `IMemoryCache` stores data in the process's memory — fast, zero latency, but not shared across multiple instances. Suitable for single-instance apps or data that can differ per instance (e.g., local rate limiting). `IDistributedCache` uses an external store (Redis, SQL Server) shared across all instances — consistent data for scaled-out deployments. Use `IMemoryCache` for simple, single-server scenarios. Use `IDistributedCache` (backed by Redis) in Kubernetes or multi-instance deployments where all nodes must see the same cached data (e.g., session state, distributed rate limiting, shared lookups).

## Q28
**Category:** Performance
**Difficulty:** Hard
**Question:** What is the ThreadPool starvation problem in .NET async code, and how does it manifest?
**Ideal Answer:** ThreadPool starvation occurs when all available threads are blocked waiting for async operations to complete (typically due to `.Result` or `.Wait()` on async methods, or synchronous I/O). New work cannot execute because no threads are free, causing timeouts and queued requests. Symptoms: high request latency under load, low CPU usage, growing thread count. Fix: always use `await` and never block on async code. Be especially cautious in middleware, filters, and library code. Use `async` all the way up the call stack. Monitor with `dotnet-counters` watching `ThreadPool Queue Length` and `ThreadPool Thread Count`.

## Q29
**Category:** Performance
**Difficulty:** Medium
**Question:** How does `System.Text.Json` differ from `Newtonsoft.Json`, and what are the performance trade-offs?
**Ideal Answer:** `System.Text.Json` is Microsoft's built-in JSON library, designed for performance: lower allocations, `Span<byte>` based parsing, and source generator support for AOT. It is significantly faster than Newtonsoft.Json for common scenarios. However, it has fewer features: no `JObject`/dynamic JSON manipulation, stricter deserialization by default (no case-insensitive matching by default, no missing member handling), and limited support for complex scenarios like polymorphism without custom converters. Newtonsoft.Json is more flexible and battle-tested for complex scenarios. For new services, prefer `System.Text.Json` with source generators; fall back to Newtonsoft.Json only for complex serialization requirements.

## Q30
**Category:** Security
**Difficulty:** Medium
**Question:** What is the difference between authentication and authorization in ASP.NET Core? How does JWT-based authentication work?
**Ideal Answer:** Authentication establishes who the user is. Authorization determines what they are allowed to do. In ASP.NET Core, `UseAuthentication()` runs first and populates `HttpContext.User` based on the provided credentials. `UseAuthorization()` then checks the user's claims against `[Authorize]` policies or attributes. JWT authentication: the client sends a signed token in the `Authorization: Bearer` header. The middleware validates the signature using the server's secret key, checks expiry and issuer/audience claims, and populates `ClaimsPrincipal`. No server-side session is needed — the token is self-contained. Short expiry + refresh tokens is the recommended pattern.

## Q31
**Category:** Security
**Difficulty:** Hard
**Question:** What are the OWASP Top 10 risks most relevant to .NET backend APIs? How would you mitigate them?
**Ideal Answer:** Key risks for .NET APIs: (1) Broken Access Control — use `[Authorize]` with resource-level checks, never trust client-supplied IDs alone. (2) Injection (SQL, command) — use parameterized queries (EF Core, Dapper), never concatenate user input into SQL. (3) Security Misconfiguration — disable Swagger in production, use HTTPS, validate JWT parameters strictly. (4) Sensitive Data Exposure — never log tokens/passwords, encrypt PII at rest. (5) Broken Authentication — short JWT lifetimes, refresh token rotation, revocation on logout. (6) SSRF — validate and allowlist any URLs the server fetches. (7) Mass Assignment — use DTOs instead of binding directly to domain entities.

## Q32
**Category:** Security
**Difficulty:** Medium
**Question:** What is refresh token rotation, and why is it important for OAuth flows?
**Ideal Answer:** Refresh token rotation means issuing a new refresh token every time the current one is used to obtain a new access token, and invalidating the old one. This limits the window of exposure if a refresh token is stolen — the attacker can only use it once before it is rotated away. Implement refresh token families: if an old (already-rotated) token is presented, this signals a possible theft — invalidate the entire family. Refresh tokens should be stored hashed in the database (not plaintext), have a long but bounded expiry (e.g., 30 days), and be tied to a device/session identifier for additional security.

## Q33
**Category:** Security
**Difficulty:** Hard
**Question:** How would you prevent SQL injection when using Dapper?
**Ideal Answer:** Always use parameterized queries. In Dapper, pass parameters as an anonymous object: `db.QueryAsync<T>("SELECT * FROM users WHERE id = @Id", new { Id = id })`. Never use string interpolation or concatenation to build SQL: `$"SELECT * FROM users WHERE id = {id}"` is vulnerable. For dynamic ORDER BY or table names (which cannot be parameterized), use a whitelist of allowed values validated in code. Dapper does not provide any automatic escaping — the developer is fully responsible for parameterization. Also avoid the `Execute` method with user-supplied multi-statement SQL.

## Q34
**Category:** Testing
**Difficulty:** Medium
**Question:** What is the difference between unit tests, integration tests, and end-to-end tests? How do you decide the right balance for a .NET API?
**Ideal Answer:** Unit tests: test a single class or function in isolation, with all dependencies mocked. Fast, no I/O. Integration tests: test multiple components together including real infrastructure (database, HTTP client). Slower but catch real interaction bugs. End-to-end tests: test the full system from the client perspective. Slowest and most fragile. For .NET APIs, the recommended balance (testing pyramid): many unit tests for business logic (command/query handlers, domain services); integration tests for database queries, middleware, and endpoint behavior using `WebApplicationFactory`; few end-to-end tests for critical user journeys only.

## Q35
**Category:** Testing
**Difficulty:** Medium
**Question:** How does `WebApplicationFactory<T>` work in ASP.NET Core integration tests, and how do you replace services for testing?
**Ideal Answer:** `WebApplicationFactory<T>` creates a test server using the real `Program.cs` startup, allowing full HTTP integration tests without deploying the app. It spins up an in-memory `TestServer` and provides an `HttpClient` for requests. To replace services, override `ConfigureWebHost` and call `builder.ConfigureServices(services => services.AddSingleton<IMyService>(mock))`. For databases, replace the `DbContext` options with an in-memory or test-container database. This approach tests the full pipeline including routing, middleware, authentication, and serialization, making it the most valuable test type for API correctness.

## Q36
**Category:** Testing
**Difficulty:** Hard
**Question:** What are the risks of using mocked databases in integration tests, and what is the recommended alternative?
**Ideal Answer:** Mocked or in-memory databases (e.g., `UseInMemoryDatabase`) do not enforce SQL constraints, do not test query performance, and differ significantly from a real database in behavior (no transactions, no migrations, no SQL-level validations). Bugs that only manifest against real Postgres or SQL Server will not be caught. The recommended alternative is Testcontainers: spin up a real Docker container of the target database per test run. The library `Testcontainers.PostgreSql` (or `MsSql`) handles container lifecycle. Tests run against a real database, migrations are applied, and the container is torn down after the test suite. This provides high confidence without requiring a shared test database.

## Q37
**Category:** Testing
**Difficulty:** Medium
**Question:** What is FluentAssertions, and how does it improve test readability compared to built-in xUnit assertions?
**Ideal Answer:** FluentAssertions provides a fluent, natural-language API for assertions. Instead of `Assert.Equal(expected, actual)`, you write `actual.Should().Be(expected)` or `result.Should().HaveCount(3).And.Contain(x => x.Name == "Test")`. When a test fails, FluentAssertions produces detailed, human-readable error messages showing the actual vs. expected values including object structure. It supports rich assertions for collections, exceptions (`action.Should().Throw<ArgumentException>()`), dates, strings, and async code. It significantly reduces the time spent debugging failing tests.

## Q38
**Category:** Azure
**Difficulty:** Medium
**Question:** What is Azure Service Bus, and how does it differ from Azure Storage Queues?
**Ideal Answer:** Azure Service Bus is an enterprise message broker supporting topics/subscriptions (pub-sub), message sessions (ordered processing), dead-letter queues, duplicate detection, transactions, and at-least-once delivery. Storage Queues are simpler, cheaper, and have higher throughput but lack topics, sessions, and transactions. Use Service Bus when you need: pub-sub fan-out, message ordering, dead-lettering with retry policies, or transactional messaging. Use Storage Queues for simple, high-volume, cost-sensitive scenarios where advanced messaging features are not needed. Service Bus integrates well with MassTransit in .NET.

## Q39
**Category:** Azure
**Difficulty:** Hard
**Question:** What is Azure Managed Identity, and how does it replace connection strings for authenticating to Azure resources?
**Ideal Answer:** Managed Identity provides an Azure AD identity to an Azure resource (e.g., App Service, AKS pod) without storing credentials. System-assigned: tied to the resource's lifecycle. User-assigned: standalone, can be shared across resources. Instead of a connection string with username/password, the application uses `DefaultAzureCredential` from the Azure SDK, which automatically resolves the identity token from the environment. Example: connect to Azure SQL with `Authentication=Active Directory Managed Identity` in the connection string, or connect to Key Vault using `new SecretClient(uri, new DefaultAzureCredential())`. Eliminates secrets from config files and rotation burdens.

## Q40
**Category:** Azure
**Difficulty:** Medium
**Question:** What is Azure Key Vault, and how do you integrate it with an ASP.NET Core application's configuration system?
**Ideal Answer:** Azure Key Vault stores secrets, certificates, and encryption keys with fine-grained access control and audit logs. Integration with ASP.NET Core: add the `Azure.Extensions.AspNetCore.Configuration.Secrets` NuGet package and call `builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential())`. Key Vault secrets are then available via the standard `IConfiguration` interface. Secret names use `--` as a separator for nested config (e.g., `ConnectionStrings--Default`). Combine with Managed Identity so no credentials are needed to access the vault itself. In development, use `dotnet user-secrets` or a local `.env` file.

## Q41
**Category:** Azure
**Difficulty:** Hard
**Question:** How does Azure API Management (APIM) complement a .NET backend API? What features does it provide?
**Ideal Answer:** APIM sits in front of backend APIs and provides: rate limiting and throttling per subscription/product, authentication (JWT validation, OAuth, client certificates) before requests reach the backend, request/response transformation via policies (add headers, rewrite URLs), caching, logging to Application Insights, versioning, and a developer portal for API documentation. Benefits: offload cross-cutting concerns from the backend, protect services from abuse, and provide a unified entry point for multiple backend services. In .NET microservices, APIM acts as an API gateway, reducing code duplication of common concerns across services.

## Q42
**Category:** Azure
**Difficulty:** Medium
**Question:** What is Azure Application Insights, and how do you configure it for distributed tracing in a .NET application?
**Ideal Answer:** Application Insights is Azure's APM service. For .NET, add `Microsoft.ApplicationInsights.AspNetCore` or use OpenTelemetry with the Azure Monitor exporter. Configure with `builder.Services.AddApplicationInsightsTelemetry()` and set the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable. It captures: request telemetry, dependency calls (HTTP, SQL), exceptions, custom events, and metrics. For distributed tracing, it uses the W3C `traceparent` header to correlate telemetry across services. The Application Map shows service dependencies. Use the `TelemetryClient` for custom events or the OpenTelemetry SDK for vendor-neutral instrumentation.

## Q43
**Category:** Azure
**Difficulty:** Hard
**Question:** What are the deployment options for a .NET API on Azure? Compare Azure App Service, Azure Container Apps, and AKS.
**Ideal Answer:** App Service: PaaS, simplest deployment model (zip deploy, GitHub Actions), auto-scaling, built-in SSL, no container orchestration knowledge needed. Best for straightforward web apps. Azure Container Apps: serverless containers, event-driven autoscaling (KEDA), Dapr integration, no Kubernetes cluster management. Best for microservices or containerized apps without deep K8s expertise. AKS (Azure Kubernetes Service): full Kubernetes control, complex workloads, custom networking, stateful sets. Highest operational cost and complexity. Best when you need advanced scheduling, custom operators, or multi-tenant isolation. For most .NET APIs: App Service for simple apps, Container Apps for container-first microservices, AKS for complex, large-scale platforms.

## Q44
**Category:** Azure
**Difficulty:** Medium
**Question:** What is Azure Blob Storage, and how do you use it from a .NET application for file uploads?
**Ideal Answer:** Azure Blob Storage stores unstructured data (files, images, logs) in containers. From .NET, use the `Azure.Storage.Blobs` SDK: create a `BlobServiceClient` (authenticated via connection string or `DefaultAzureCredential`), get a `BlobContainerClient`, and upload with `blobClient.UploadAsync(stream, overwrite: true)`. For large file uploads from clients, prefer SAS (Shared Access Signature) tokens: generate a time-limited URL on the server and let the client upload directly to Blob Storage, bypassing your API. This avoids streaming large files through the server, reducing memory pressure and egress costs.

## Q45
**Category:** Azure
**Difficulty:** Hard
**Question:** What is Azure Functions, and when would you choose it over a hosted `BackgroundService` in ASP.NET Core?
**Ideal Answer:** Azure Functions is a serverless compute service that executes code in response to triggers (HTTP, timer, Service Bus, Blob, Event Grid). It scales to zero (no cost when idle), scales out automatically, and has no server management. Choose Functions when: the workload is event-driven and sporadic, you want cost-per-execution billing, or you want isolated deployability per function. Choose `BackgroundService` in ASP.NET Core when: you need tight integration with the same app's DI container, the work is continuous (e.g., polling), or you need predictable latency without cold start. Functions cold starts can be mitigated with Premium plan or pre-warming.

## Q46
**Category:** Databases
**Difficulty:** Hard
**Question:** What is the difference between a clustered and a non-clustered index in SQL databases? How does index design affect .NET API performance?
**Ideal Answer:** A clustered index defines the physical order of rows on disk — there is one per table (the primary key by default). A non-clustered index is a separate structure with pointers to the actual rows. Queries that filter or sort on non-clustered index columns avoid full table scans. For .NET API performance: identify slow queries via EF Core logging or `EXPLAIN ANALYZE` (Postgres). Add non-clustered indexes on columns used in WHERE, JOIN ON, and ORDER BY clauses. Covering indexes (include all selected columns) eliminate key lookups. Over-indexing hurts write performance. EF Core migrations support `.HasIndex()` and `.IncludeProperties()` for composite and covering indexes.

## Q47
**Category:** Databases
**Difficulty:** Medium
**Question:** What is a database transaction, and how do you manage transactions in EF Core and Dapper within the same unit of work?
**Ideal Answer:** A transaction groups multiple operations into an atomic unit — all succeed or all roll back. In EF Core, `SaveChangesAsync()` wraps all pending changes in a transaction automatically. For explicit transactions: `using var tx = await context.Database.BeginTransactionAsync()`. To share a transaction between EF Core and Dapper: get the underlying `DbConnection` and `DbTransaction` from EF Core (`context.Database.GetDbConnection()`, `context.Database.CurrentTransaction.GetDbTransaction()`) and pass them to Dapper's `Execute`/`Query` overloads. This ensures both ORM and raw SQL participate in the same transaction.

## Q48
**Category:** Databases
**Difficulty:** Hard
**Question:** What is connection pooling in the context of .NET database access, and how does it work with Npgsql/PostgreSQL?
**Ideal Answer:** Connection pooling reuses established database connections instead of creating a new TCP connection per request, which is expensive. Npgsql has a built-in connection pool per connection string. When `OpenAsync()` is called, Npgsql returns a connection from the pool; when `CloseAsync()` (or `Dispose()`) is called, it returns it to the pool — not actually closing the TCP connection. Key settings: `Maximum Pool Size` (default 100), `Minimum Pool Size`, `Connection Idle Lifetime`. With EF Core's scoped `DbContext`, each request gets one connection from the pool and returns it at end of scope. Misconfiguring pool size relative to Postgres's `max_connections` can cause connection exhaustion.

## Q49
**Category:** Messaging
**Difficulty:** Hard
**Question:** What is MassTransit, and how does it simplify message-based communication in .NET microservices?
**Ideal Answer:** MassTransit is an open-source .NET messaging abstraction that supports RabbitMQ, Azure Service Bus, Amazon SQS, and others via a consistent API. It handles: consumer registration (DI-integrated), retry policies, circuit breakers, dead-letter queues, sagas (state machine orchestration for distributed workflows), request/response over messaging, and the Outbox Pattern with EF Core. Instead of writing broker-specific code, you define `IConsumer<TMessage>` and register it with `services.AddMassTransit(x => x.AddConsumer<MyConsumer>())`. It significantly reduces boilerplate for reliable messaging and integrates with ASP.NET Core's hosted services.

## Q50
**Category:** Messaging
**Difficulty:** Medium
**Question:** What is the difference between point-to-point messaging and publish/subscribe? Give a concrete example with Azure Service Bus.
**Ideal Answer:** Point-to-point (queue): a message is sent to a named queue and consumed by exactly one receiver. Suitable for work distribution (e.g., a job queue where one worker processes each order). Publish/subscribe (topic + subscription): a publisher sends a message to a topic; multiple subscriptions (each with their own filter rules) receive independent copies. Suitable for event-driven integration (e.g., `OrderPlaced` event consumed by the Billing service and the Notification service independently). In Azure Service Bus: use `QueueClient` for P2P, use `TopicClient` + `SubscriptionClient` for pub-sub. MassTransit abstracts both patterns.

## Q51
**Category:** Design Patterns
**Difficulty:** Medium
**Question:** What is the Mediator pattern, and how does MediatR implement it in ASP.NET Core?
**Ideal Answer:** The Mediator pattern centralizes communication between objects through a single mediator, reducing direct coupling. MediatR implements this by having all commands and queries implement `IRequest<TResponse>`. Handlers implement `IRequestHandler<TRequest, TResponse>` and are discovered via DI. The endpoint calls `mediator.Send(new MyCommand(...))` without knowing which handler processes it. This decouples the HTTP layer from the business logic, makes handlers independently testable, and supports cross-cutting behaviors (pipeline behaviors) for logging, validation, and caching inserted between the request and handler.

## Q52
**Category:** Design Patterns
**Difficulty:** Medium
**Question:** What is the Decorator pattern, and how can it be applied in .NET using DI to add cross-cutting behavior?
**Ideal Answer:** The Decorator wraps an object to add behavior without modifying it. In .NET DI, register decorators using Scrutor (`services.Decorate<IMyService, LoggingMyService>()`) or manually by resolving the inner service and wrapping it. Example: a `CachingQueryHandler<T>` that wraps a real handler and returns cached results. Pipeline behaviors in MediatR achieve the same effect for requests. Benefits: single responsibility (each decorator does one thing), open/closed (add behavior without modifying existing code), testable in isolation. Common use cases: logging, caching, retry, authorization checks.

## Q53
**Category:** Design Patterns
**Difficulty:** Hard
**Question:** What is the Specification pattern, and how does it help with complex query composition in EF Core?
**Ideal Answer:** A Specification encapsulates query criteria as an object. Instead of passing raw predicates everywhere, you define `class ActiveUserSpecification : Specification<User>` that builds an `Expression<Func<User, bool>>`. The repository or query handler applies the specification: `context.Users.Where(spec.Criteria)`. Benefits: reusable, combinable (AND/OR operators), testable without a database. Libraries like Ardalis.Specification provide a base class and `ISpecificationEvaluator` that also handles `Include`, `OrderBy`, and pagination. This avoids query logic leaking into handlers and controllers.

## Q54
**Category:** Design Patterns
**Difficulty:** Medium
**Question:** What is the Factory pattern? When would you use `IServiceProvider` as a factory vs. a dedicated factory class?
**Ideal Answer:** The Factory pattern encapsulates object creation. In .NET, `IServiceProvider.GetRequiredService<T>()` acts as a service locator/factory but is an antipattern when used in business logic (hides dependencies). A dedicated factory interface (`INotificationFactory`) makes dependencies explicit and is easier to test. Use `IServiceProvider` as a factory only in infrastructure code (e.g., creating scoped services inside a singleton like a hosted service: `scope = provider.CreateScope()`). Use a dedicated factory class for creating domain objects or when the type to create depends on runtime data.

## Q55
**Category:** Logging & Observability
**Difficulty:** Medium
**Question:** What are structured logs, and why are they preferable to plain text logs in a cloud-native application?
**Ideal Answer:** Structured logs store log data as key-value pairs (JSON) rather than free-form strings. Instead of `"User 42 logged in from 1.2.3.4"`, you emit `{ "event": "UserLogin", "userId": 42, "ip": "1.2.3.4" }`. This enables: filtering and querying by specific fields (e.g., all events for `userId = 42`), aggregations and metrics dashboards, and correlation of events across services. In .NET, Serilog and Microsoft.Extensions.Logging support structured logging. With Serilog, use message templates: `Log.Information("User {UserId} logged in from {IpAddress}", userId, ip)`. The values are captured as structured properties, not just formatted into the string.

## Q56
**Category:** Logging & Observability
**Difficulty:** Hard
**Question:** What is OpenTelemetry, and how does it unify tracing, metrics, and logging in a .NET application?
**Ideal Answer:** OpenTelemetry (OTel) is a vendor-neutral observability framework providing a single set of APIs and SDKs for traces, metrics, and logs. In .NET: add `OpenTelemetry.Extensions.Hosting`, configure `TracerProvider` with `AddAspNetCoreInstrumentation()`, `AddHttpClientInstrumentation()`, `AddEntityFrameworkCoreInstrumentation()`, and export to Jaeger, Zipkin, or Azure Monitor via `AddOtlpExporter()`. Traces propagate via W3C `traceparent` headers across services, enabling end-to-end request correlation. Metrics are exported to Prometheus. This replaces proprietary SDKs (Application Insights SDK, Datadog tracer) with a single instrumentation layer, allowing you to swap backends without code changes.

## Q57
**Category:** Logging & Observability
**Difficulty:** Medium
**Question:** What is a correlation ID, and how would you propagate it across services and log it consistently?
**Ideal Answer:** A correlation ID is a unique identifier attached to a request and passed across all service calls and log entries, enabling you to trace a single user request across multiple services and log lines. Implementation in ASP.NET Core: middleware reads `X-Correlation-ID` from incoming headers (or generates a new one), stores it in a scoped service or `Activity.Current`, and adds it to outgoing `HttpClient` requests via a `DelegatingHandler`. In Serilog, use `LogContext.PushProperty("CorrelationId", id)` via `UseSerilogRequestLogging` enrichment so every log line in that request scope includes the ID. OpenTelemetry's `TraceId` serves the same purpose.

## Q58
**Category:** API Design
**Difficulty:** Medium
**Question:** What is ProblemDetails (RFC 7807), and how do you implement it consistently across an ASP.NET Core API?
**Ideal Answer:** ProblemDetails is an HTTP standard format for machine-readable error responses, including `type`, `title`, `status`, `detail`, and `instance` fields. In ASP.NET Core 7+, call `builder.Services.AddProblemDetails()` and use `Results.Problem(...)` in minimal APIs or `Problem(...)` in controllers. For unhandled exceptions, configure `UseExceptionHandler` to return `ProblemDetails` via `IExceptionHandler`. This gives clients a consistent error contract regardless of the endpoint, and allows extension properties (e.g., `errors` for validation failures). It is preferable to ad-hoc `{ "message": "..." }` responses that vary per endpoint.

## Q59
**Category:** API Design
**Difficulty:** Medium
**Question:** What is API versioning, and how would you implement it in an ASP.NET Core application?
**Ideal Answer:** API versioning allows evolving an API without breaking existing clients. Common strategies: URL path (`/api/v1/users`), query string (`?api-version=1.0`), or header (`Api-Version: 1.0`). Use the `Asp.Versioning.Http` NuGet package: call `builder.Services.AddApiVersioning(options => options.DefaultApiVersion = new ApiVersion(1, 0))`. Annotate endpoints or controllers with `[ApiVersion("1.0")]`. Deprecate old versions with `[ApiVersion("1.0", Deprecated = true)]`. Document with Swagger using `Asp.Versioning.Mvc.ApiExplorer`. Prefer header-based versioning for APIs that must keep clean URLs; URL-based versioning is more visible and easier to test manually.

## Q60
**Category:** API Design
**Difficulty:** Hard
**Question:** What is HATEOAS, and is it practical to implement in modern .NET REST APIs?
**Ideal Answer:** HATEOAS (Hypermedia as the Engine of Application State) means API responses include links to related actions and resources, allowing clients to navigate the API dynamically without hardcoding URLs. Example: a `GET /orders/1` response includes `"links": [{"rel": "cancel", "href": "/orders/1/cancel", "method": "POST"}]`. In theory, it decouples client and server. In practice: most real-world APIs (including major cloud providers) do not implement full HATEOAS because clients need to understand resource semantics regardless, and the overhead of link generation is not justified. It is rarely implemented beyond Level 2 Richardson Maturity. A well-documented OpenAPI spec is typically more practical.

## Q61
**Category:** Concurrency
**Difficulty:** Hard
**Question:** What is the difference between optimistic and pessimistic concurrency, and when would you choose each in a .NET API?
**Ideal Answer:** Optimistic concurrency: assume conflicts are rare. Read data, process, then on save check that the row hasn't changed (via `rowversion`/`xmin`). If changed, throw `DbUpdateConcurrencyException` and let the client retry. Low overhead, no locks held. Pessimistic concurrency: lock the row on read so no other transaction can modify it (`SELECT FOR UPDATE` in Postgres). Guarantees no conflict but holds locks, reducing throughput and risking deadlocks. Use optimistic for most web API scenarios (low contention, short transactions). Use pessimistic for high-contention scenarios where the cost of conflict resolution is high (e.g., seat reservation, inventory deduction).

## Q62
**Category:** Concurrency
**Difficulty:** Hard
**Question:** How does `SemaphoreSlim` differ from `lock` for async code, and when would you use each?
**Ideal Answer:** `lock` is synchronous — it blocks the thread while waiting to acquire the monitor. Using `lock` in async code is dangerous because you cannot await inside a lock block, and blocking can cause ThreadPool starvation. `SemaphoreSlim` supports async waiting: `await semaphore.WaitAsync()` yields the thread while waiting, allowing other work to proceed. Use `lock` for synchronous, very short critical sections (e.g., updating an in-memory collection). Use `SemaphoreSlim(1, 1)` as an async mutex for protecting shared state in async code. Use `SemaphoreSlim(N, N)` to limit concurrent access to a resource (e.g., max 5 concurrent HTTP calls to an external API).

## Q63
**Category:** Concurrency
**Difficulty:** Medium
**Question:** What is `CancellationToken`, and how should it be propagated in a .NET API?
**Ideal Answer:** `CancellationToken` signals that an operation should be cancelled (e.g., client disconnects, request timeout). In ASP.NET Core, the framework injects a `CancellationToken` into endpoint/action parameters automatically — it is cancelled when the client disconnects. Propagate it through every async call: database queries (`QueryAsync(..., cancellationToken: ct)`), HTTP client calls, file I/O, and background work. Never ignore it. Create linked tokens to add a timeout: `using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct); cts.CancelAfter(TimeSpan.FromSeconds(5))`. Proper propagation prevents resource leaks and database query execution after a client has disconnected.

## Q64
**Category:** Dependency Injection
**Difficulty:** Hard
**Question:** What is the Service Locator antipattern, and how does it differ from legitimate use of `IServiceProvider`?
**Ideal Answer:** The Service Locator pattern means calling `serviceProvider.GetService<T>()` from within business logic to resolve dependencies at runtime rather than declaring them as constructor parameters. It is an antipattern because: dependencies are hidden (no explicit constructor contract), the class is harder to test (must configure the container), and it couples business logic to the DI infrastructure. Legitimate uses of `IServiceProvider`: creating scoped services inside a singleton (e.g., `provider.CreateScope()` in a `BackgroundService`), factory methods in infrastructure code, and middleware that needs to resolve a request-scoped service. In domain and application code, always use constructor injection.

## Q65
**Category:** Dependency Injection
**Difficulty:** Medium
**Question:** What are the limitations of constructor injection in .NET DI, and how do you handle optional or conditional dependencies?
**Ideal Answer:** Constructor injection requires all dependencies to be registered and available. For optional dependencies: use `IServiceProvider.GetService<T>()` (returns null if not registered) instead of `GetRequiredService<T>()`. Alternatively, declare the parameter as nullable and use `null` as the default. For conditional dependencies (different implementations per runtime condition): use a factory pattern — inject `IServiceProvider` or a `Func<T>` factory. For open generics (e.g., `IRepository<T>`), register with `services.AddScoped(typeof(IRepository<>), typeof(Repository<>))`. Avoid over-engineering: most services should have a single concrete implementation.

## Q66
**Category:** .NET Runtime
**Difficulty:** Hard
**Question:** How does the .NET garbage collector work? What are Gen 0, Gen 1, and Gen 2, and what is the Large Object Heap?
**Ideal Answer:** .NET uses a generational GC. Gen 0: newly allocated short-lived objects. Collected frequently, fast (milliseconds). Gen 1: objects that survived Gen 0. Gen 2: long-lived objects (singletons, caches). Collected infrequently, can pause the application. Large Object Heap (LOH): objects ≥ 85,000 bytes are allocated here and only collected with Gen 2. LOH is not compacted by default, causing fragmentation. To minimize GC pressure: prefer `Span<T>` and `stackalloc` for temporary buffers, use `ArrayPool<byte>.Shared` for large arrays, avoid frequent large string concatenations (use `StringBuilder`), and avoid keeping large collections in Gen 2 if possible.

## Q67
**Category:** .NET Runtime
**Difficulty:** Medium
**Question:** What is the difference between `IDisposable` and `IAsyncDisposable`? When should you implement each?
**Ideal Answer:** `IDisposable.Dispose()` releases unmanaged resources synchronously. `IAsyncDisposable.DisposeAsync()` releases resources asynchronously (e.g., flushing an async stream, closing a network connection gracefully). Implement `IDisposable` when your class holds references to resources like file handles, unmanaged memory, or DB connections. Implement `IAsyncDisposable` when cleanup involves I/O. Implement both if the class can be disposed in either context. Use `await using` for `IAsyncDisposable`. Never call `Dispose()` on an object still tracked by the DI container — let the container manage lifetime. The ASP.NET Core DI container calls `DisposeAsync()` for scoped/transient services that implement it.

## Q68
**Category:** .NET Runtime
**Difficulty:** Hard
**Question:** What is Native AOT in .NET, and what constraints does it impose on application code?
**Ideal Answer:** Native AOT (Ahead-of-Time compilation) compiles .NET code to a native binary before deployment, eliminating the JIT compiler at runtime. Benefits: faster startup time (milliseconds vs. hundreds of milliseconds), lower memory footprint, smaller deployment (no runtime needed), better container cold start. Constraints: no runtime code generation (no `Reflection.Emit`, no expression trees compiled at runtime), limited reflection (must use source generators instead), no dynamic assembly loading, no `Type.GetType(string)` patterns. ASP.NET Core minimal APIs and `System.Text.Json` with source generators are designed for AOT compatibility. EF Core has limited AOT support. Best suited for Lambda functions, microservices, and CLI tools.

## Q69
**Category:** .NET Runtime
**Difficulty:** Medium
**Question:** What is the difference between `string` interning, `StringBuilder`, and `string.Create` in terms of memory allocation?
**Ideal Answer:** `string` is immutable — every concatenation creates a new string object. String interning pools identical literals to reuse the same reference (`string.Intern`), useful for reducing allocations with repeated values but risky for memory leaks if overused. `StringBuilder` accumulates characters in a mutable buffer and only allocates the final string on `ToString()` — ideal for building strings in loops. `string.Create(length, state, spanAction)` is the most allocation-efficient: it allocates exactly one string and writes into it via a `Span<char>`, avoiding intermediate copies. Use `StringBuilder` for variable-length building; `string.Create` for fixed-length, performance-critical paths.

## Q70
**Category:** Cloud Native
**Difficulty:** Hard
**Question:** What is the 12-Factor App methodology, and how do the most relevant factors apply to a .NET microservice?
**Ideal Answer:** 12-Factor defines best practices for cloud-native apps. Most relevant for .NET: (1) Config: store config in environment variables, never in code. Use `IConfiguration` + `AddEnvironmentVariables()`. (2) Backing services: treat DB, queue, cache as attached resources via config, not hardcoded. (3) Processes: be stateless — no in-memory sessions, no sticky sessions. (4) Logs: treat logs as event streams — write to stdout/stderr, not files. Use structured logging. (5) Disposability: fast startup (< 5s), graceful shutdown on SIGTERM. Implement `IHostedService.StopAsync`. (6) Dev/prod parity: use Docker Compose locally to match production. (7) Dependencies: declare all dependencies in `.csproj`, no implicit system-level dependencies.

## Q71
**Category:** Cloud Native
**Difficulty:** Medium
**Question:** What is health checking in ASP.NET Core, and how would you configure liveness vs. readiness probes for Kubernetes?
**Ideal Answer:** ASP.NET Core provides `AddHealthChecks()` and `MapHealthChecks()`. Liveness probe: checks if the process is alive and not deadlocked. Should be very lightweight — just return Healthy. Map to `/health/live` and check only self: `.AddCheck("self", () => HealthCheckResult.Healthy())`. Readiness probe: checks if the app is ready to receive traffic (DB connected, dependencies up). Map to `/health/ready` and include all checks: `.AddDbContextCheck<AppDbContext>()`. In Kubernetes, configure `livenessProbe` on `/health/live` and `readinessProbe` on `/health/ready`. Separate them to avoid killing a pod that is just temporarily unable to reach the DB.

## Q72
**Category:** Cloud Native
**Difficulty:** Hard
**Question:** How would you implement circuit breaking and retry policies in a .NET application calling external HTTP APIs?
**Ideal Answer:** Use Polly (or `Microsoft.Extensions.Http.Resilience` in .NET 8+). Retry policy: retry on transient errors (5xx, network timeouts) with exponential backoff + jitter to avoid thundering herd. Circuit breaker: after N consecutive failures, open the circuit for a duration and fail fast without calling the service — prevents cascading failures and allows recovery. Configure via `services.AddHttpClient<MyClient>().AddResilienceHandler(...)` or `AddTransientHttpErrorPolicy`. Key considerations: do not retry non-idempotent operations (POST) unless the endpoint is idempotent. Log circuit state transitions. Combine with timeout policies. In distributed systems, resilience is not optional.

## Q73
**Category:** Cloud Native
**Difficulty:** Medium
**Question:** What is containerization, and what are the best practices for writing a Dockerfile for a .NET API?
**Ideal Answer:** Best practices for .NET Dockerfiles: (1) Use multi-stage builds — SDK image for build, runtime image for final (reduces image size from ~700MB to ~200MB). (2) Restore packages before copying source code so Docker caches the restore layer. (3) Use the `mcr.microsoft.com/dotnet/aspnet` image (not SDK) for the runtime stage. (4) Run as a non-root user for security. (5) Set `ASPNETCORE_URLS=http://+:8080` to avoid running on port 80. (6) Use `.dockerignore` to exclude `bin/`, `obj/`, `.git/`. (7) For AOT-compiled apps, use the `chiseled` Ubuntu base image for minimal attack surface.

## Q74
**Category:** Soft Skills & Process
**Difficulty:** Medium
**Question:** How do you approach a pull request review as a senior developer? What do you look for beyond correctness?
**Ideal Answer:** Beyond correctness: (1) Design: does the change introduce unnecessary complexity? Could it be simpler? Is it consistent with the existing architecture? (2) Security: any injection risks, missing authorization checks, secrets in logs? (3) Performance: N+1 queries, unbounded collections, missing indexes, sync over async? (4) Testability: are the changes tested? Are tests testing behavior or implementation details? (5) Error handling: are errors handled gracefully or silently swallowed? (6) Observability: are important events logged with context? (7) Naming and readability: would a newcomer understand this in 6 months? (8) Breaking changes: does this affect existing clients or contracts?

## Q75
**Category:** Soft Skills & Process
**Difficulty:** Medium
**Question:** How do you handle technical debt in a fast-moving product? How do you decide when to refactor vs. ship?
**Ideal Answer:** Technical debt is inevitable; the key is managing it intentionally. Strategies: (1) Track debt as explicit backlog items, not invisible knowledge. (2) Apply the Boy Scout Rule: leave code slightly better than you found it. (3) Refactor in the context of a related feature — do not create unrelated cleanup PRs that stall review. (4) Use the Strangler Fig pattern for large refactors: build new behavior alongside old and migrate incrementally. (5) Distinguish between deliberate debt (conscious shortcut with a plan) and reckless debt (no plan). Ship when the debt does not block the current goal and is tracked. Refactor when debt is actively slowing the team or introducing bugs.

## Q76
**Category:** Soft Skills & Process
**Difficulty:** Hard
**Question:** How would you approach migrating a legacy .NET Framework monolith to .NET 8+ without a full rewrite?
**Ideal Answer:** Strangler Fig pattern: incrementally replace parts of the monolith rather than rewriting everything at once. Steps: (1) Identify a bounded slice (e.g., a single subsystem) that can be extracted with minimal dependencies. (2) Create a new ASP.NET Core project alongside the monolith. (3) Use an API Gateway or reverse proxy (YARP, NGINX) to route traffic for that slice to the new service. (4) Share the database initially (anti-pattern but pragmatic); extract to separate schema/DB over time. (5) Migrate shared libraries to `netstandard2.0` first for compatibility, then to `net8.0`. Avoid rewriting business logic from scratch — port it, keep tests green.

## Q77
**Category:** API Design
**Difficulty:** Medium
**Question:** What is the difference between REST and gRPC? When would you choose gRPC for a .NET service?
**Ideal Answer:** REST uses HTTP/1.1 with JSON, is human-readable, and is the standard for public APIs. gRPC uses HTTP/2 with Protocol Buffers (binary), providing: lower payload size, bidirectional streaming, code generation from `.proto` contracts, and strict API contracts. In .NET, use `Grpc.AspNetCore`. Choose gRPC for: high-throughput internal service communication, polyglot microservices needing strongly-typed contracts, bidirectional streaming (e.g., real-time telemetry), and mobile clients where bandwidth matters. Choose REST for: public APIs, browser clients (gRPC-Web adds complexity), or when human-readability and tooling (Postman, Swagger) are priorities.

## Q78
**Category:** API Design
**Difficulty:** Hard
**Question:** How would you design a pagination API? Compare offset-based vs. cursor-based pagination and their trade-offs.
**Ideal Answer:** Offset-based (`?page=2&pageSize=20`): simple to implement and supports jumping to arbitrary pages. Drawback: inconsistent results when data is inserted/deleted during pagination (rows shift), and `OFFSET N` in SQL becomes slow for large N (DB scans all preceding rows). Cursor-based: the response includes a cursor (opaque token encoding the last seen ID or sort key). The next request passes `?cursor=...` to fetch rows after that cursor. Benefits: consistent results regardless of concurrent writes, O(1) seek time with the right index. Drawback: cannot jump to arbitrary pages, harder to implement. Use offset for admin grids with moderate data; use cursor for feeds, event streams, and large tables.

## Q79
**Category:** Architecture
**Difficulty:** Hard
**Question:** What is Event Sourcing, and what are the trade-offs compared to a traditional state-based persistence model?
**Ideal Answer:** Event Sourcing persists the sequence of domain events that led to the current state, rather than the current state itself. Reconstruct state by replaying events. Benefits: full audit log, time-travel queries, ability to project state into multiple read models, natural fit for CQRS. Trade-offs: queries require projections (read models) — you cannot simply `SELECT * FROM orders`. Schema evolution of events is complex. Eventual consistency between write and read models. Rebuilding state from many events can be slow without snapshots. High operational complexity. Best suited for domains with inherent audit requirements (finance, compliance) or where event history is a first-class feature. Overkill for simple CRUD applications.

## Q80
**Category:** Architecture
**Difficulty:** Medium
**Question:** What is the Anti-Corruption Layer (ACL) pattern in DDD, and when is it needed?
**Ideal Answer:** An ACL is a translation layer between two bounded contexts or systems with different models. When integrating with a legacy system, external API, or third-party service, the external model's concepts should not leak into your domain model. The ACL translates external DTOs, events, or structures into your domain's language. Example: an external payment gateway returns `TransactionStatus` values in its own format; the ACL maps these to your domain's `PaymentStatus` enum. Without an ACL, external model changes force changes throughout your domain. Implement as adapters, mappers, or a dedicated translation service. Always use an ACL when the external model is outside your control.

## Q81
**Category:** Azure
**Difficulty:** Hard
**Question:** What is Azure Event Grid, and how does it differ from Azure Service Bus and Azure Event Hubs?
**Ideal Answer:** Event Grid: reactive event routing for Azure resource events and custom events. Low latency, HTTP push (webhooks), serverless. Best for reacting to Azure resource changes (Blob created, resource group deleted) or lightweight custom events. No ordering or replay. Service Bus: enterprise message broker. Ordered processing, dead-lettering, transactions, sessions. Best for reliable, ordered command processing between services. Event Hubs: high-throughput event streaming (millions of events/sec). Retains events for 24h–90 days, supports consumer groups for parallel processing, integrates with Apache Kafka protocol. Best for telemetry, log aggregation, real-time analytics pipelines. The choice depends on volume, ordering needs, and consumer model.

## Q82
**Category:** Azure
**Difficulty:** Medium
**Question:** What is Azure Redis Cache, and how would you implement a distributed cache in an ASP.NET Core API?
**Ideal Answer:** Azure Cache for Redis is a managed Redis instance. Integrate with ASP.NET Core via `services.AddStackExchangeRedisCache(options => options.Configuration = config["Redis:ConnectionString"])`. Use `IDistributedCache.GetAsync/SetAsync` with byte array serialization, or wrap it with a typed helper using `System.Text.Json`. For strongly typed caching, use `StackExchange.Redis` directly for richer operations (sets, sorted sets, pub-sub). Common patterns: cache-aside (check cache, on miss load from DB and populate cache with expiry), write-through (update DB and cache atomically). Set appropriate TTLs. Use distributed locking (`RedLock`) for cache stampede prevention on high-traffic keys.

## Q83
**Category:** C# Language
**Difficulty:** Medium
**Question:** What is the difference between `abstract` and `virtual` methods in C#? When would you use an interface vs. an abstract class?
**Ideal Answer:** `virtual`: provides a default implementation that derived classes can override. `abstract`: declares a method with no implementation; derived classes must override it. An abstract class can have both abstract and concrete members, cannot be instantiated, and can hold state. Interface: pure contract with no state (fields), supports multiple inheritance, and since C# 8, can have default implementations. Use an abstract class when: sharing implementation across a hierarchy, need a common base with partial implementation, or need protected members. Use an interface when: defining a contract that multiple unrelated types will implement, or enabling dependency injection with multiple implementations.

## Q84
**Category:** C# Language
**Difficulty:** Hard
**Question:** What are expression trees in C#, and how does EF Core use them to translate LINQ to SQL?
**Ideal Answer:** An expression tree represents code as data — a tree of `Expression` nodes that can be inspected and transformed at runtime. When you write `context.Users.Where(u => u.Email == email)`, the lambda is captured as `Expression<Func<User, bool>>` (not compiled to a delegate). EF Core's LINQ provider traverses this expression tree and translates it into SQL: `WHERE email = @email`. This is why EF Core can translate LINQ to SQL but cannot translate arbitrary C# method calls (methods not understood by the provider throw `InvalidOperationException`). Expression trees are also used in ORM mapping, dynamic query builders (Specification pattern), and source generators.

## Q85
**Category:** C# Language
**Difficulty:** Medium
**Question:** What are nullable reference types in C# 8+, and how do they help prevent `NullReferenceException`?
**Ideal Answer:** Nullable reference types (NRT) introduce compile-time null analysis. When enabled (`<Nullable>enable</Nullable>`), reference types are non-nullable by default — `string name` guarantees non-null, `string? name` explicitly allows null. The compiler emits warnings when: a nullable value is used without null checking, or when a non-nullable parameter/property could receive null. This catches `NullReferenceException` bugs at compile time rather than at runtime. Migrate code by enabling NRT per-file first, adding `?` annotations and null checks. NRT does not change runtime behavior — it is purely a compile-time analysis tool.

## Q86
**Category:** Design Patterns
**Difficulty:** Medium
**Question:** What is the Observer pattern, and how is it implemented in .NET? Give an example beyond `event` delegates.
**Ideal Answer:** The Observer pattern defines a one-to-many dependency where observers are notified of state changes. In .NET: (1) `event` delegates — the simplest form; publishers fire events and subscribers register handlers. (2) `IObservable<T>`/`IObserver<T>` — the reactive push-based model used by Reactive Extensions (Rx.NET). (3) `INotifyPropertyChanged` — used in WPF/MAUI data binding. (4) MediatR notifications — `INotificationHandler<T>` for in-process domain event dispatching. (5) Channels (`System.Threading.Channels`) — a producer/consumer queue for decoupled communication within a process. Choose based on whether you need synchronous/async notification, backpressure, or fan-out.

## Q87
**Category:** Performance
**Difficulty:** Hard
**Question:** What is `ArrayPool<T>` and `MemoryPool<T>`, and when should they be used instead of `new T[]`?
**Ideal Answer:** `ArrayPool<T>.Shared` provides a pool of reusable arrays, avoiding repeated heap allocation and GC pressure for temporary large buffers. Rent with `ArrayPool<byte>.Shared.Rent(minLength)` and return with `ArrayPool<byte>.Shared.Return(buffer, clearArray: true)`. The returned array may be larger than requested — always track the actual size used. `MemoryPool<T>` is the abstract, async-safe counterpart returning `IMemoryOwner<T>` which implements `IDisposable` for safe return via `using`. Use cases: HTTP request body reading, binary protocol parsing, file I/O, any code that allocates short-lived byte arrays in hot paths. Never forget to return the rented array — failure to return causes pool exhaustion.

## Q88
**Category:** Security
**Difficulty:** Hard
**Question:** What is PKCE, and why is it required for OAuth 2.0 authorization code flows in native/SPA clients?
**Ideal Answer:** PKCE (Proof Key for Code Exchange) prevents authorization code interception attacks in public clients (native apps, SPAs) that cannot securely store a client secret. Flow: the client generates a random `code_verifier`, computes `code_challenge = BASE64URL(SHA256(code_verifier))`, and includes the challenge in the authorization request. When exchanging the code for a token, the client sends the original `code_verifier`. The server verifies it matches the challenge. An attacker who intercepts the authorization code cannot exchange it without the original `code_verifier`. PKCE is mandatory for public clients per OAuth 2.1 and recommended for all authorization code flows regardless of client type.

## Q89
**Category:** Testing
**Difficulty:** Hard
**Question:** What is mutation testing, and how does it complement code coverage metrics?
**Ideal Answer:** Mutation testing evaluates test suite quality by introducing small, intentional bugs (mutations) into the code (e.g., changing `>` to `>=`, flipping a boolean) and verifying that existing tests catch them (kill the mutant). If a mutation survives (tests still pass with a bug in the code), the test suite has a gap. Code coverage only measures whether a line was executed, not whether the test asserts meaningful behavior. A test can achieve 100% coverage while asserting nothing. Mutation testing provides a much higher signal. Tools: Stryker.NET for C#. High mutation scores indicate tests are genuinely verifying behavior, not just exercising code paths.

## Q90
**Category:** Databases
**Difficulty:** Hard
**Question:** What is a database migration strategy for zero-downtime deployments? How do you handle breaking schema changes?
**Ideal Answer:** Zero-downtime migrations require backward compatibility between the old and new code versions. Strategy: (1) Expand: add new column as nullable (no default required, old code ignores it, new code writes to it). (2) Migrate: backfill data in batches to avoid table locks. (3) Contract: after all instances run the new code, add constraints or drop old columns. Never add a NOT NULL column without a default in a single deployment. Never rename columns or tables in one step — add new, migrate, drop old over multiple deployments. Use feature flags to decouple deployment from feature activation. EF Core migrations should be applied separately from code deployment (e.g., as an init container in Kubernetes).

## Q91
**Category:** Architecture
**Difficulty:** Hard
**Question:** What is the Saga pattern for distributed transactions? Compare choreography vs. orchestration.
**Ideal Answer:** A Saga is a sequence of local transactions coordinated to achieve a distributed transaction without a two-phase commit. Each step has a compensating transaction for rollback. Choreography: each service listens for events and reacts, emitting further events. Decentralized, no single point of failure, but hard to visualize and debug the overall flow. Orchestration: a central coordinator (saga orchestrator) calls each service in sequence and issues compensating commands on failure. Easier to understand and monitor, but the orchestrator can become a bottleneck. In .NET, MassTransit Sagas implement orchestration as state machines. Use sagas for: order fulfillment, multi-step payment flows, and any process spanning multiple services.

## Q92
**Category:** Databases
**Difficulty:** Medium
**Question:** What is the difference between Dapper and EF Core, and when would you use each in the same project?
**Ideal Answer:** EF Core: full ORM with change tracking, migrations, LINQ-to-SQL translation, relationship management. Ideal for write operations (commands) where you need change tracking, concurrency control, and schema management. Dapper: micro-ORM — thin wrapper over ADO.NET that maps SQL query results to objects. No change tracking, no LINQ translation. Faster and simpler for read operations. Ideal pattern in CQRS: command handlers use EF Core for writes (leveraging change tracking and migrations), query handlers use Dapper for reads (optimized SQL, projections, JOINs, aggregations). Share the same `IDbConnection`/`DbContext` connection. This gives you the correctness of EF Core for writes and the performance of raw SQL for reads.

## Q93
**Category:** C# Language
**Difficulty:** Hard
**Question:** What is the difference between `Task.WhenAll` and `Task.WhenAny`? What are the risks of using `Task.WhenAll` without proper error handling?
**Ideal Answer:** `Task.WhenAll` completes when all tasks finish — if any task faults, it rethrows the first exception; other exceptions are stored in `AggregateException.InnerExceptions`. `Task.WhenAny` completes as soon as the first task completes (or faults). Risk with `WhenAll`: if you `await` it without a try/catch, only the first exception is surfaced; the others are silently ignored, potentially hiding failures. To inspect all exceptions: catch `AggregateException` or check each task individually after `WhenAll`. Another risk: if tasks are not bounded, launching thousands of tasks simultaneously can exhaust the ThreadPool or connection pool. Use `SemaphoreSlim` to throttle concurrency when processing large collections.

## Q94
**Category:** Azure
**Difficulty:** Medium
**Question:** What is Azure Cosmos DB, and what trade-offs does it have compared to Azure SQL Database for a .NET backend?
**Ideal Answer:** Cosmos DB is a globally distributed, multi-model NoSQL database with guaranteed single-digit millisecond latency and configurable consistency levels (strong to eventual). Strengths: horizontal scale-out, geo-replication, schema flexibility, multiple APIs (SQL, Mongo, Cassandra). Trade-offs vs. Azure SQL: no ACID transactions across partitions (limited to single partition or multi-document within a partition in newer versions), no arbitrary JOINs, schema design must align with access patterns, and RU (Request Unit) billing model can be expensive and hard to predict. Use Cosmos DB for: high-volume, globally distributed workloads with well-defined access patterns. Use Azure SQL for: relational data, complex queries, reporting, and when ACID guarantees are critical.

## Q95
**Category:** Architecture
**Difficulty:** Hard
**Question:** What is the strangler fig pattern, and how would you apply it to decompose a monolith incrementally?
**Ideal Answer:** Named after a tree that slowly surrounds its host, the Strangler Fig pattern incrementally replaces a legacy system by building new functionality in a new system alongside the old one, redirecting traffic slice by slice. Steps: (1) Identify a slice (a feature or domain boundary) to extract. (2) Build the new service implementing that slice. (3) Use a facade or reverse proxy (YARP, NGINX, API Gateway) to route traffic for that slice to the new service. (4) Verify correctness, then decommission the old code. (5) Repeat. Benefits: no big-bang rewrite, continuous delivery, reversible at each step. Key challenge: database sharing during transition — use shared DB initially, extract schema ownership over time with ACL translation layers.

## Q96
**Category:** C# Language
**Difficulty:** Medium
**Question:** What are `init` only setters in C# 9, and how do they support immutable object construction?
**Ideal Answer:** `init` setters can only be called during object initialization (in the constructor or an object initializer), not after. They allow: `var obj = new MyClass { Name = "test" }` (readable syntax) while preventing mutation after construction (`obj.Name = "other"` is a compile error). Unlike `readonly` fields, `init` properties can be used in object initializers and `with` expressions for records. They are ideal for DTOs and value objects that must be immutable post-construction while still supporting readable initialization syntax. Combined with `required` modifier (C# 11), they enforce that the property is set at initialization time.

## Q97
**Category:** Design Patterns
**Difficulty:** Hard
**Question:** What is the Strategy pattern, and how does it compare to using polymorphism (inheritance) in .NET?
**Ideal Answer:** The Strategy pattern defines a family of algorithms, encapsulates each, and makes them interchangeable at runtime via composition. In .NET: inject `IPaymentStrategy` into a handler; at runtime inject `StripePaymentStrategy` or `PayPalPaymentStrategy`. With inheritance (polymorphism), the algorithm is baked into the class hierarchy — switching behavior requires a different subclass, and multiple behaviors require multiple inheritance levels. Strategy uses composition: the behavior is a separate, injectable object. Benefits: easier to add new strategies without modifying existing classes (open/closed), easier to test each strategy in isolation, and supports runtime selection. Prefer composition over inheritance for behavior variation.

## Q98
**Category:** .NET Runtime
**Difficulty:** Hard
**Question:** What is the difference between `Thread`, `Task`, `ThreadPool`, and `async/await` in .NET concurrency?
**Ideal Answer:** `Thread`: an OS-level thread. Expensive (1MB stack by default). Use only for long-running blocking work. `ThreadPool`: a pool of reusable threads managed by the CLR. Tasks are queued to it. Expanding the pool is slow. `Task`: represents an asynchronous operation — it may run on a ThreadPool thread or complete without a thread (pure I/O). `async/await`: syntactic sugar over `Task` that generates a state machine; the method suspends at each `await`, freeing the thread for other work, and resumes when the awaited operation completes. For I/O-bound work (database, HTTP), `async/await` is ideal — no thread is blocked while waiting. For CPU-bound work, use `Task.Run` to offload to the ThreadPool without blocking the request thread.

## Q99
**Category:** API Design
**Difficulty:** Medium
**Question:** What is FluentValidation, and how do you integrate it with an ASP.NET Core API for request validation?
**Ideal Answer:** FluentValidation provides a fluent API for building validation rules in dedicated validator classes, separating validation logic from domain and controller code. Define `class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>` with rules like `RuleFor(x => x.Email).NotEmpty().EmailAddress()`. Register with `services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>()`. In minimal APIs or MediatR, validate manually: `var result = validator.Validate(command); if (!result.IsValid) return Results.ValidationProblem(result.ToDictionary())`. For controllers, use the `FluentValidation.AspNetCore` integration to hook into `ModelState` automatically. Compared to Data Annotations, FluentValidation is more expressive, supports conditional rules, cross-property validation, and is easier to unit test.

## Q100
**Category:** Architecture
**Difficulty:** Hard
**Question:** How do you design for observability from day one in a .NET microservice? What is the difference between monitoring and observability?
**Ideal Answer:** Monitoring answers "is the system up?" with predefined metrics and alerts. Observability answers "why is the system behaving this way?" by exploring unknown failure modes through telemetry (logs, metrics, traces). Design for observability: (1) Structured logging with correlation IDs and request context (Serilog + enrichers). (2) Distributed tracing with OpenTelemetry — auto-instrument HTTP, DB, and messaging; propagate trace context. (3) Metrics: request rate, error rate, latency percentiles (p50, p95, p99), queue depth — expose via Prometheus endpoint or OTLP. (4) Health checks with meaningful checks (not just "alive"). (5) Alerting on SLO breaches, not just server health. Observability is a first-class engineering concern, not an afterthought.
