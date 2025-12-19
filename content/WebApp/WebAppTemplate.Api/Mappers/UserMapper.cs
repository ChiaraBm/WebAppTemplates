using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using WebAppTemplate.Shared.Http.Requests.Users;
using WebAppTemplate.Shared.Http.Responses.Users;
using WebAppTemplate.Api.Database.Entities;

namespace WebAppTemplate.Api.Mappers;

[Mapper]
[SuppressMessage("Mapper", "RMG020:No members are mapped in an object mapping")]
[SuppressMessage("Mapper", "RMG012:No members are mapped in an object mapping")]
public static partial class UserMapper
{
    public static partial IQueryable<UserResponse> ProjectToResponse(this IQueryable<User> users);

    public static partial UserResponse MapToResponse(User user);

    public static partial void Merge([MappingTarget] User user, UpdateUserRequest request);
    
    public static partial User MapToUser(CreateUserRequest request);
}