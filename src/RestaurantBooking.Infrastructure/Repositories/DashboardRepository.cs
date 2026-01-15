using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Infrastructure.Persistence;
using RestaurantBooking.Domain.ValueObjects;

namespace RestaurantBooking.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly RestaurantBookingDbContext _context;

    public DashboardRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardMetrics> GetMetricsAsync(int restaurantId)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Queries
        var reservationsToday = await _context.Reservations
            .Where(r => r.RestaurantId == restaurantId && 
                        r.ReservationDate >= today && 
                        r.ReservationDate < tomorrow)
            .ToListAsync();

        var upcoming = await _context.Reservations
            .Where(r => r.RestaurantId == restaurantId && r.ReservationDate > DateTime.UtcNow)
            .OrderBy(r => r.ReservationDate)
            .Take(5)
            .Select(r => new ReservationDto(
                r.Id, 
                r.Code, 
                r.CustomerName, 
                r.ReservationDate, 
                r.NumberOfGuests, 
                r.Status.ToString()))
            .ToListAsync();

        return new DashboardMetrics(
            TotalReservationsToday: reservationsToday.Count,
            PendingCount: reservationsToday.Count(r => r.Status == ReservationStatus.Pending),
            ConfirmedCount: reservationsToday.Count(r => r.Status == ReservationStatus.Confirmed),
            CheckedInCount: reservationsToday.Count(r => r.Status == ReservationStatus.CheckedIn),
            OccupancyRate: reservationsToday.Count > 0 ? (reservationsToday.Sum(r => r.NumberOfGuests) / 50.0) * 100 : 0,
            UpcomingReservations: upcoming
        );
    }
}
