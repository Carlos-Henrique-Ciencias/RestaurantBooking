using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Infrastructure.Persistence;
using RestaurantBooking.Infrastructure.Repositories;

namespace RestaurantBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Agora pegamos a string de conexão do arquivo de configuração (appsettings)
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<RestaurantBookingDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IReservationRepository, ReservationRepository>();
services.AddScoped<IDashboardRepository, DashboardRepository>();

        return services;
    }
}
