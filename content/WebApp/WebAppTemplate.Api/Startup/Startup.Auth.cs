using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WebAppTemplate.Api.Configuration;
using WebAppTemplate.Api.Services;

namespace WebAppTemplate.Api.Startup;

public static partial class Startup
{
    private static void AddAuth(WebApplicationBuilder builder)
    {
        var oidcOptions = new OidcOptions();
        builder.Configuration.GetSection("WebApp:Oidc").Bind(oidcOptions);
        
        builder.Services.AddScoped<UserAuthService>();

        builder.Services.AddAuthentication("Session")
            .AddCookie("Session", null, options =>
            {
                options.Events.OnSigningIn += async context =>
                {
                    var authService = context
                        .HttpContext
                        .RequestServices
                        .GetRequiredService<UserAuthService>();

                    var result = await authService.SyncAsync(context.Principal);

                    if (result)
                        context.Properties.IsPersistent = true;
                    else
                        context.Principal = new ClaimsPrincipal();
                };

                options.Events.OnValidatePrincipal += async context =>
                {
                    var authService = context
                        .HttpContext
                        .RequestServices
                        .GetRequiredService<UserAuthService>();

                    var result = await authService.ValidateAsync(context.Principal);

                    if (!result)
                        context.RejectPrincipal();
                };
                
                options.Cookie = new CookieBuilder()
                {
                    Name = "token",
                    Path = "/",
                    IsEssential = true,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest
                };
            })
            .AddOpenIdConnect("OIDC", "OpenID Connect", options =>
            {
                options.Authority = oidcOptions.Authority;
                options.RequireHttpsMetadata = oidcOptions.RequireHttpsMetadata;

                var scopes = oidcOptions.Scopes ?? ["openid", "email", "profile"];

                foreach (var scope in scopes)
                    options.Scope.Add(scope);

                options.ResponseType = oidcOptions.ResponseType;
                options.ClientId = oidcOptions.ClientId;
                options.ClientSecret = oidcOptions.ClientSecret;
                
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "preferred_username");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");

                options.GetClaimsFromUserInfoEndpoint = true;
            });

        builder.Services.AddAuthorization();
    }

    private static void UseAuth(WebApplication application)
    {
        application.UseAuthentication();
        application.UseAuthorization();
    }
}