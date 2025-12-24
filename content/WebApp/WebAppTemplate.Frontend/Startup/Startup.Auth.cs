using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAppTemplate.Frontend.Services;

namespace WebAppTemplate.Frontend.Startup;

public static partial class Startup
{
    public static void AddAuth(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<AuthenticationStateProvider, RemoteAuthProvider>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
    }
}