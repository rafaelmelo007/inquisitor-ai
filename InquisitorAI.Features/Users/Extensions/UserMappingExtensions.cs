using InquisitorAI.Features.Users.Domain;
using InquisitorAI.Features.Users.Dtos;

namespace InquisitorAI.Features.Users.Extensions;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User entity) =>
        new(
            entity.Id,
            entity.Email,
            entity.DisplayName,
            entity.AvatarUrl,
            entity.Provider.ToString(),
            entity.CreatedAt);
}
