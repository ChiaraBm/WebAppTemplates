using System.Text.Json.Serialization;
using WebAppTemplate.Shared.Http.Requests.Users;
using WebAppTemplate.Shared.Http.Responses;
using WebAppTemplate.Shared.Http.Responses.Auth;
using WebAppTemplate.Shared.Http.Responses.Users;

namespace WebAppTemplate.Shared.Http;

[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(ClaimResponse[]))]
[JsonSerializable(typeof(SchemeResponse[]))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(PagedData<UserResponse>))]
public partial class SerializationContext : JsonSerializerContext
{
    
}