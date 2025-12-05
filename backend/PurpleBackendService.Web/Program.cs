using Purple.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Initialize();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.AddMapControllers();
app.Run();