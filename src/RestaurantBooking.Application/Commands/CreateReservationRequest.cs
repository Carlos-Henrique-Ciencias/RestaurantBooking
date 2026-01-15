namespace RestaurantBooking.Application.Commands;

public class CreateReservationRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public int NumberOfGuests { get; set; }
    public int RestaurantId { get; set; }
}
