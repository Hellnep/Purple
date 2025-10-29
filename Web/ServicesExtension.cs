using Microsoft.EntityFrameworkCore;

using Purple.Common.Database.Context.Sqlite;

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
    }
}