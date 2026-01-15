using RestaurantBooking.Domain.ValueObjects;

namespace RestaurantBooking.Application.Queries;

public class GetAllReservationsQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? CustomerName { get; set; }
    public List<ReservationStatus>? Statuses { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public string SortBy { get; set; } = "ReservationDate";
    public string SortDirection { get; set; } = "asc";
}
