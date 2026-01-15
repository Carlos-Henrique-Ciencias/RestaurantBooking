using RestaurantBooking.Domain.ValueObjects;

namespace RestaurantBooking.Application.DTOs;

public record ReservationDto(
    int Id,
    Guid Code,
    string CustomerName,
    DateTime ReservationDate,
    int NumberOfGuests,
    string Status
);
