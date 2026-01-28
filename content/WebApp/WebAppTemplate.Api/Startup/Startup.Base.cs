using Microsoft.Extensions.Logging.Console;
using WebAppTemplate.Shared.Http;
using WebAppTemplate.Api.Helpers;

namespace WebAppTemplate.Api.Startup;

public static partial class Startup
{
    private static void AddBase(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.TypeInfoResolverChain.Add(SerializationContext.Default);
        });
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options =>
        {
            options.FormatterName = nameof(AppConsoleFormatter);
        });
        builder.Logging.AddConsoleFormatter<AppConsoleFormatter, ConsoleFormatterOptions>();

        builder.Services.AddMemoryCache();
    }

    private static void UseBase(WebApplication application)
    {
        application.UseBlazorFrameworkFiles();
        application.UseStaticFiles();

        application.UseRouting();
    }

    private static void MapBase(WebApplication application)
    {
        application.MapControllers();

        application.MapFallbackToFile("index.html");
    }
}