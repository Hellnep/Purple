using Purple.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Initialize();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.Run();


