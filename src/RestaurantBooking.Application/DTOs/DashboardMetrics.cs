namespace RestaurantBooking.Application.DTOs;

public record DashboardMetrics(
    int TotalReservationsToday,
    int PendingCount,
    int ConfirmedCount,
    int CheckedInCount,
    double OccupancyRate,
    List<ReservationDto> UpcomingReservations
);
