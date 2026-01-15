namespace RestaurantBooking.Domain.ValueObjects;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    Completed,
    Cancelled,
    NoShow
}
