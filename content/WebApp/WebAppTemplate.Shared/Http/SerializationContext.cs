using System.Text.Json.Serialization;
using WebAppTemplate.Shared.Http.Requests.Users;
using WebAppTemplate.Shared.Http.Responses;
using WebAppTemplate.Shared.Http.Responses.Auth;
using WebAppTemplate.Shared.Http.Responses.Users;

namespace WebAppTemplate.Shared.Http;

[JsonSerializable(typeof(CreateUserDto))]
[JsonSerializable(typeof(UpdateUserDto))]
[JsonSerializable(typeof(ClaimDto[]))]
[JsonSerializable(typeof(SchemeDto[]))]
[JsonSerializable(typeof(UserDto))]
[JsonSerializable(typeof(PagedData<UserDto>))]
public partial class SerializationContext : JsonSerializerContext
{
    
}