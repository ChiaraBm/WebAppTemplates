using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using WebAppTemplate.Shared.Http.Responses.Auth;

namespace WebAppTemplate.Frontend.Services;

public class RemoteAuthProvider : AuthenticationStateProvider
{
    private readonly ILogger<RemoteAuthProvider> Logger;
    private readonly HttpClient HttpClient;

    public RemoteAuthProvider(ILogger<RemoteAuthProvider> logger, HttpClient httpClient)
    {
        Logger = logger;
        HttpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var claimResponses = await HttpClient.GetFromJsonAsync<ClaimResponse[]>(
                "api/auth/claims", Constants.SerializerOptions
            );

            var claims = claimResponses!.Select(claim => new Claim(claim.Type, claim.Value));

            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(claims, "remote"))
            );
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode != HttpStatusCode.Unauthorized)
                Logger.LogError(e, "An api error occured while requesting claims from api");
        }
        catch (Exception e)
        {
            Logger.LogError(e, "An unhandled error occured while requesting claims from api");
        }

        return new AuthenticationState(new ClaimsPrincipal());
    }
}