namespace WebAppTemplate.Api.Startup;

public static partial class Startup
{
    public static void AddWebAppTemplate(this WebApplicationBuilder builder)
    {
        AddBase(builder);
        AddAuth(builder);
        AddDatabase(builder);
    }

    public static void UseWebAppTemplate(this WebApplication application)
    {
        UseBase(application);
        UseAuth(application);
        
        MapBase(application);
    }
}