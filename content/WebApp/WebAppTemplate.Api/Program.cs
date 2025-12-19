using WebAppTemplate.Api.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.AddWebAppTemplate();

var app = builder.Build();

app.UseWebAppTemplate();

app.Run();