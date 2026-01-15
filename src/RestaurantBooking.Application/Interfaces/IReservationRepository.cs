using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Domain.ValueObjects;

namespace RestaurantBooking.Application.Interfaces;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation);
    Task<Reservation?> GetByIdAsync(int id);
Task UpdateAsync(Reservation reservation);
    
    // Novo método poderoso para listagem
    Task<(List<Reservation> Items, int TotalCount)> GetAllAsync(
        string? customerName, 
        List<ReservationStatus>? statuses, 
        DateTime? dateStart, 
        DateTime? dateEnd, 
        int page, 
        int pageSize,
        string sortBy,
        string sortDirection);
}
