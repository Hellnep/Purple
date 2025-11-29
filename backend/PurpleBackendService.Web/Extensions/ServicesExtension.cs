using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Core;
using PurpleBackendService.Core.Repository;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Infrastructure.Sqlite;

namespace Purple.Web;

internal static class ServicesExtension
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

        services.AddDbContext<PurpleOcean>(
            options => options.UseSqlite("Data Source=PurpleOcean.db")
        );
        
        services.AddTransient<IRepository<Product>, ProductRepository>();
        services.AddTransient<IProductService, ProductService>();

        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddTransient<ICustomerService, CustomerService>();

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
}