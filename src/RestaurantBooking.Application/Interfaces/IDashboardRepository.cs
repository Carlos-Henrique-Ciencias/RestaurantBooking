using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Application.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardMetrics> GetMetricsAsync(int restaurantId);
}
