using System.Data;
using Dapper;
using InquisitorAI.Features.Shared;
using InquisitorAI.Features.Users.Dtos;

namespace InquisitorAI.Features.Users.Queries;

public record GetCurrentUserQuery(long UserId) : IQuery<UserDto?>;

public class GetCurrentUserHandler(IDbConnection db)
    : IQueryHandler<GetCurrentUserQuery, UserDto?>
{
    public async Task<UserDto?> HandleAsync(GetCurrentUserQuery query, CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                id AS Id,
                email AS Email,
                display_name AS DisplayName,
                avatar_url AS AvatarUrl,
                provider AS Provider,
                created_at AS CreatedAt
            FROM inq_users
            WHERE id = @UserId
            """;

        return await db.QuerySingleOrDefaultAsync<UserDto>(sql, new { query.UserId });
    }
}
