using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using WebAppTemplate.Shared.Http.Requests.Users;
using WebAppTemplate.Shared.Http.Responses.Users;

namespace WebAppTemplate.Frontend.Mappers;

[Mapper]
[SuppressMessage("Mapper", "RMG020:No members are mapped in an object mapping")]
[SuppressMessage("Mapper", "RMG012:No members are mapped in an object mapping")]
public static partial class UserMapper
{
    public static partial UpdateUserDto ToUpdate(UserDto dto);
}