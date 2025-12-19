using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppTemplate.Shared.Http.Requests;
using WebAppTemplate.Shared.Http.Requests.Users;
using WebAppTemplate.Shared.Http.Responses;
using WebAppTemplate.Shared.Http.Responses.Users;
using WebAppTemplate.Api.Database;
using WebAppTemplate.Api.Database.Entities;
using WebAppTemplate.Api.Mappers;

namespace WebAppTemplate.Api.Http.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : Controller
{
    private readonly DatabaseRepository<User> UserRepository;

    public UsersController(DatabaseRepository<User> userRepository)
    {
        UserRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PagedData<UserResponse>>> GetAsync(
        [FromQuery] int startIndex,
        [FromQuery] int length,
        [FromQuery] FilterOptions? filterOptions
    )
    {
        // Validation
        if (startIndex < 0)
            return Problem("Invalid start index specified", statusCode: 400);

        if (length is < 1 or > 100)
            return Problem("Invalid length specified");

        var query = UserRepository
            .Query();

        // Filters
        if (filterOptions != null)
        {
            foreach (var filterOption in filterOptions.Filters)
            {
                query = filterOption.Key switch
                {
                    nameof(Database.Entities.User.Email) =>
                        query.Where(user => EF.Functions.ILike(user.Email, $"%{filterOption.Value}%")),

                    nameof(Database.Entities.User.Username) =>
                        query.Where(user => EF.Functions.ILike(user.Username, $"%{filterOption.Value}%")),

                    _ => query
                };
            }
        }

        // Pagination
        var data = await query
            .ProjectToResponse()
            .Skip(startIndex)
            .Take(length)
            .ToArrayAsync();

        var total = await query.CountAsync();

        return new PagedData<UserResponse>(data, total);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetAsync([FromRoute] int id)
    {
        var user = await UserRepository
            .Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return Problem("No user with this id found", statusCode: 404);

        return UserMapper.MapToResponse(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateAsync([FromBody] CreateUserRequest request)
    {
        var user = UserMapper.MapToUser(request);
        user.InvalidateTimestamp = DateTimeOffset.UtcNow.AddMinutes(-1);

        var finalUser = await UserRepository.AddAsync(user);

        return UserMapper.MapToResponse(finalUser);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<UserResponse>> UpdateAsync([FromRoute] int id, [FromBody] UpdateUserRequest request)
    {
        var user = await UserRepository
            .Query()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (user == null)
            return Problem("No user with this id found", statusCode: 404);
        
        UserMapper.Merge(user, request);
        await UserRepository.UpdateAsync(user);

        return UserMapper.MapToResponse(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var user = await UserRepository
            .Query()
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
            return Problem("No user with this id found", statusCode: 404);

        await UserRepository.RemoveAsync(user);
        return NoContent();
    }
}