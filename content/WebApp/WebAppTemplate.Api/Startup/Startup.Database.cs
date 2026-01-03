using WebAppTemplate.Api.Configuration;
using WebAppTemplate.Api.Database;
using WebAppTemplate.Api.Services;

namespace WebAppTemplate.Api.Startup;

public static partial class Startup
{
    private static void AddDatabase(WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<DatabaseOptions>().BindConfiguration("WebAppTemplate:Database");

        builder.Services.AddDbContext<DataContext>();
        builder.Services.AddScoped(typeof(DatabaseRepository<>));
        builder.Services.AddHostedService<DbMigrationService>();
    }
}