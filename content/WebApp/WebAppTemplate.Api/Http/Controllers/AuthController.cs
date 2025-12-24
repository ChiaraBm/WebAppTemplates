using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppTemplate.Shared.Http.Responses.Auth;

namespace WebAppTemplate.Api.Http.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthenticationSchemeProvider SchemeProvider;
    private readonly string[] AllowedSchemes = [OpenIdConnectDefaults.AuthenticationScheme];

    public AuthController(IAuthenticationSchemeProvider schemeProvider)
    {
        SchemeProvider = schemeProvider;
    }

    [HttpGet]
    public async Task<ActionResult<SchemeResponse[]>> GetSchemesAsync()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();

        return schemes
            .Where(scheme => !string.IsNullOrWhiteSpace(scheme.DisplayName) && AllowedSchemes.Contains(scheme.Name))
            .Select(scheme => new SchemeResponse(scheme.Name, scheme.DisplayName!))
            .ToArray();
    }

    [HttpGet("{schemeName:alpha}")]
    public async Task<ActionResult> StartAsync([FromRoute] string schemeName)
    {
        var scheme = await SchemeProvider.GetSchemeAsync(schemeName);

        if (scheme == null || string.IsNullOrWhiteSpace(scheme.DisplayName) || !AllowedSchemes.Contains(scheme.Name))
            return Problem("Invalid authentication scheme name", statusCode: 400);

        return Challenge(new AuthenticationProperties()
        {
            RedirectUri = "/"
        }, scheme.Name);
    }

    [Authorize]
    [HttpGet("claims")]
    public Task<ActionResult<ClaimResponse[]>> GetClaimsAsync()
    {
        var result = User.Claims
            .Select(claim => new ClaimResponse(claim.Type, claim.Value))
            .ToArray();

        return Task.FromResult<ActionResult<ClaimResponse[]>>(result);
    }

    [HttpGet("logout")]
    public Task<ActionResult> LogoutAsync()
    {
        return Task.FromResult<ActionResult>(
            SignOut(new AuthenticationProperties()
            {
                RedirectUri = "/"
            })
        );
    }
}