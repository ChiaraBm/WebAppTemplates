using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace WebAppTemplate.Frontend.Startup;

public static partial class Startup
{
    public static void AddWebAppTemplate(this WebAssemblyHostBuilder builder)
    {
        AddBase(builder);
        AddAuth(builder);
    }

    public static void UseWebAppTemplate(this WebAssemblyHost application)
    {
        
    }
}