using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Domain.ValueObjects;
using RestaurantBooking.Infrastructure.Persistence;

namespace RestaurantBooking.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly RestaurantBookingDbContext _context;

    public ReservationRepository(RestaurantBookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _context.Reservations.FindAsync(id);
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<Reservation> Items, int TotalCount)> GetAllAsync(
        string? customerName,
        List<ReservationStatus>? statuses,
        DateTime? dateStart,
        DateTime? dateEnd,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection)
    {
        var query = _context.Reservations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(customerName))
            query = query.Where(r => r.CustomerName.ToLower().Contains(customerName.ToLower()));

        if (statuses != null && statuses.Any())
            query = query.Where(r => statuses.Contains(r.Status));

        if (dateStart.HasValue) query = query.Where(r => r.ReservationDate >= dateStart.Value);
        if (dateEnd.HasValue) query = query.Where(r => r.ReservationDate <= dateEnd.Value);

        var totalCount = await query.CountAsync();

        query = sortDirection?.ToLower() == "desc" 
            ? query.OrderByDescending(r => EF.Property<object>(r, sortBy ?? "ReservationDate"))
            : query.OrderBy(r => EF.Property<object>(r, sortBy ?? "ReservationDate"));

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }
}
