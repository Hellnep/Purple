using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

using PurpleBackendService.Infrastucture.Repository;
using PurpleBackendService.Infrastucture.Services;
using PurpleBackendService.Infrastructure.Sqlite;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Core.Services;
using PurpleBackendService.Domain.Interfaces.Repositories;

namespace PurpleBackendService.Web.Configure
{
    internal static class DependencyInjectionExtension
    {
        /// <summary>
        /// Extention method for services collection
        /// </summary>
        /// <param name="services">Input services</param>
        /// <returns></returns>
        public static void Initialize(this IServiceCollection services)
        {
            services.AddOpenApi();
            services.AddControllers();
            services.AddProblemDetails();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Purple Content Service",
                    Version = "v1"
                });
            });

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

        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PurpleOcean>(
                options => options.UseSqlite(configuration["ConnectionStrings:DefaultConnection"])
            );
        }

        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IImageService, ImageService>();

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUserService, UserService>();
        }

        public static void AddMapControllers(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.MapControllers();
        }

        /// <summary>
        /// Global catching all error on process work an app.
        /// </summary>
        /// <param name="app">Current web application</param>
        public static void AddCaptionThrow(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async handler =>
                {
                    var exception = handler.Features.Get<IExceptionHandlerFeature>()?.Error;
                    var (status, title) = exception switch
                    {
                        ArgumentNullException => (StatusCodes.Status500InternalServerError, "Resource not found"),
                        _ => (StatusCodes.Status500InternalServerError, "Server Error")
                    };

                    handler.Response.StatusCode = status;

                    await handler.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = status,
                        Title = title,
                        Detail = app.Environment.IsDevelopment() ? exception?.Message : null,
                        Instance = handler.Request.Path
                    });
                });
            });
        }
    }
}
