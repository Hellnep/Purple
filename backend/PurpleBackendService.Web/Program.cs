#pragma warning disable IDE0007 // Использование неявного типа
using PurpleBackendService.Web.Configure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Initialize();
builder.Services.AddDatabase(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.AddMapControllers();
app.AddCaptionThrow();
app.Run();
