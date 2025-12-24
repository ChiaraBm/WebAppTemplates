using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShadcnBlazor;
using ShadcnBlazor.Extras;
using WebAppTemplate.Frontend.UI;

namespace WebAppTemplate.Frontend.Startup;

public static partial class Startup
{
    private static void AddBase(WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        
        builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        
        builder.Services.AddShadcnBlazor();
        builder.Services.AddShadcnBlazorExtras();
    }
}