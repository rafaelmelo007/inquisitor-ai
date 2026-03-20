# .NET Developer Interview

## Question 1

**Category:** C# Fundamentals
**Difficulty:** Medium
**Question:** Explain the difference between `abstract class` and `interface` in C#. When would you choose one over the other? How has this changed with default interface methods introduced in C# 8?
**Ideal Answer:** An abstract class can contain implementation, state (fields), and constructors, while an interface traditionally only defines a contract with no implementation. You choose an abstract class when you need shared state or a common base implementation among related types, and an interface when you want to define a capability that unrelated types can implement. C# 8 introduced default interface methods, which allow interfaces to provide default implementations, blurring the line somewhat. However, interfaces still cannot hold instance state, so abstract classes remain the better choice when shared fields or constructor logic are needed.

## Question 2

**Category:** ASP.NET Core
**Difficulty:** Hard
**Question:** Describe the ASP.NET Core middleware pipeline. How does request processing flow through middleware components, and what is the significance of the order in which middleware is registered?
**Ideal Answer:** The ASP.NET Core middleware pipeline is a series of components that each get a chance to handle an incoming HTTP request and the outgoing response. Each middleware can perform logic before and after calling the next component in the pipeline. The order of registration in `Program.cs` (or `Startup.Configure`) is critical because it determines the execution order: for example, authentication middleware must run before authorization, and exception-handling middleware should be registered first so it can catch exceptions from all subsequent middleware. The pipeline forms a Russian-doll model where each middleware wraps the next.

## Question 3

**Category:** Entity Framework Core
**Difficulty:** Medium
**Question:** What is the difference between eager loading, lazy loading, and explicit loading in Entity Framework Core? What are the performance implications of each approach?
**Ideal Answer:** Eager loading uses `.Include()` and `.ThenInclude()` to load related data in a single query upfront, which avoids the N+1 problem but may fetch more data than needed. Lazy loading loads related entities on first access via proxy objects, which is convenient but can cause N+1 query issues if not carefully managed. Explicit loading uses `.Entry().Reference().Load()` or `.Entry().Collection().Load()` to manually load related data on demand. For most API scenarios, eager loading is preferred because it is predictable and keeps query counts low. Lazy loading should be used cautiously, especially in web applications where accidental property access in serialization can trigger unexpected queries.

## Question 4

**Category:** Architecture
**Difficulty:** Hard
**Question:** What is the CQRS pattern and how does it differ from traditional CRUD? What benefits does separating reads and writes provide in a .NET application?
**Ideal Answer:** CQRS (Command Query Responsibility Segregation) separates the read model from the write model. Commands mutate state and may use an ORM like Entity Framework Core for transactional writes, while queries are read-only and can use lightweight tools like Dapper for performance. Unlike traditional CRUD where a single model handles both reads and writes, CQRS allows each side to be optimized independently. Benefits include simpler read queries (tailored DTOs, no change tracking overhead), better scalability (reads and writes can scale separately), and clearer intent in the codebase. The tradeoff is increased complexity from maintaining separate models and handlers.

## Question 5

**Category:** Security
**Difficulty:** Medium
**Question:** How does JWT-based authentication work in ASP.NET Core? What are the key components of a JWT, and how do you protect against common JWT vulnerabilities such as token theft and replay attacks?
**Ideal Answer:** JWT (JSON Web Token) authentication works by issuing a signed token after successful login, which the client includes in the Authorization header for subsequent requests. A JWT has three parts: header (algorithm and type), payload (claims like user ID, roles, and expiration), and signature (cryptographic proof of integrity). ASP.NET Core validates the token using `AddAuthentication().AddJwtBearer()` middleware, checking the signature, issuer, audience, and expiration. To mitigate token theft, use short-lived access tokens paired with refresh tokens stored securely, enforce HTTPS, and avoid storing JWTs in localStorage (prefer httpOnly cookies). Replay attacks can be mitigated with token expiration, one-time-use refresh tokens, and token revocation lists.
