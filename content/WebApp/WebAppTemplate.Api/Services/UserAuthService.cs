using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WebAppTemplate.Api.Configuration;
using WebAppTemplate.Api.Database;
using WebAppTemplate.Api.Database.Entities;

namespace WebAppTemplate.Api.Services;

public class UserAuthService
{
    private readonly IMemoryCache MemoryCache;
    private readonly ILogger<UserAuthService> Logger;
    private readonly DatabaseRepository<User> UserRepository;
    private readonly IOptions<SessionsOptions> SessionsOptions;

    private const string UserIdClaim = "UserId";
    private const string IssuedAtClaim = "IssuedAt";
    private const string CacheKeyFormat = $"{nameof(UserAuthService)}_{nameof(ValidateAsync)}_{{0}}";

    public UserAuthService(
        DatabaseRepository<User> userRepository,
        ILogger<UserAuthService> logger,
        IOptions<SessionsOptions> sessionsOptions,
        IMemoryCache memoryCache
    )
    {
        UserRepository = userRepository;
        Logger = logger;
        SessionsOptions = sessionsOptions;
        MemoryCache = memoryCache;
    }

    public async Task<bool> SyncAsync(ClaimsPrincipal? principal)
    {
        if (principal is null)
            return false;

        var username = principal.FindFirstValue(ClaimTypes.Name);
        var email = principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            Logger.LogWarning("Unable to sync user to database as name and/or email claims are missing");
            return false;
        }

        // We use email as the primary identifier here
        var user = await UserRepository
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email);

        if (user == null) // Sync user if not already existing in the database
        {
            user = await UserRepository.AddAsync(new User()
            {
                Username = username,
                Email = email,
                InvalidateTimestamp = DateTimeOffset.UtcNow.AddMinutes(-1)
            });
        }
        else // Update properties of existing user
        {
            user.Username = username;

            await UserRepository.UpdateAsync(user);
        }

        principal.Identities.First().AddClaims([
            new Claim(UserIdClaim, user.Id.ToString()),
            new Claim(IssuedAtClaim, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        ]);

        return true;
    }

    public async Task<bool> ValidateAsync(ClaimsPrincipal? principal)
    {
        // Ignore malformed claims principal
        if (principal is not { Identity.IsAuthenticated: true })
            return false;

        var userIdString = principal.FindFirstValue(UserIdClaim);

        if (!int.TryParse(userIdString, out var userId))
            return false;

        var issuedAtString = principal.FindFirstValue(IssuedAtClaim);

        if (!long.TryParse(issuedAtString, out var issuedAtUnix))
            return false;

        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(issuedAtUnix).ToUniversalTime();
        
        // Handle caching
        var cacheKey = string.Format(CacheKeyFormat, userId);

        if (!MemoryCache.TryGetValue<UserSession>(cacheKey, out var session))
        {
            session = await UserRepository
                .Query()
                .AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => new UserSession(user.InvalidateTimestamp))
                .FirstOrDefaultAsync();

            if (session == null)
                return false;
            
            MemoryCache.Set(cacheKey, session, TimeSpan.FromMinutes(SessionsOptions.Value.CacheMinutes));
        }

        // If the issued at timestamp is greater than the token validation timestamp,
        // everything is fine. If not, it means that the token should be invalidated
        // as it is too old
        
        if(session == null)
            return false;

        return issuedAt > session.InvalidateTimestamp;
    }

    private record UserSession(DateTimeOffset InvalidateTimestamp);
}