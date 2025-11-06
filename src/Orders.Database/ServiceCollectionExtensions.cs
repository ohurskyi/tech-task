using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Orders.Database;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrdersDbContext(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<OrdersDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgOpt => npgOpt.UseNodaTime())
                .UseSnakeCaseNamingConvention();
        });
    }
}