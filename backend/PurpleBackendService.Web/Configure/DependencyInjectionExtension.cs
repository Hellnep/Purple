using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PurpleBackendService.Core;
using PurpleBackendService.Core.Repository;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Infrastructure.Sqlite;

namespace Purple.Web;

internal static class DependencyInjectionExtension
{
    /// <summary>
    /// Расширенный метод инициализации сервисов с данной сборке.
    /// </summary>
    /// <param name="services">Сервис, который должен инициализировать службы.</param>
    /// <returns></returns>
    public static void Initialize(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Purple Content Service",
                Version = "v1"
            });
        });

        services.AddDbContext<PurpleOcean>(
            options => options.UseSqlite("Data Source=PurpleOcean.db")
        );
        
        services.AddCoreServices();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = customize =>
            {
                customize.ProblemDetails.Extensions["traceId"] = customize.HttpContext.TraceIdentifier;
                customize.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                customize.ProblemDetails.Instance = $"{customize.HttpContext.Request.Method}, {customize.HttpContext.Request.Path}";
            };
        });
    }

    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<IRepository<Product>, ProductRepository>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();

        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<ICustomerService, CustomerService>();
    }

    public static void AddMapControllers(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseDeveloperExceptionPage();
        
        app.UseRouting();
        app.MapControllers();
    }
}