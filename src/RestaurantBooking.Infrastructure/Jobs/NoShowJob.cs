using Microsoft.EntityFrameworkCore;
using RestaurantBooking.Domain.ValueObjects;
using RestaurantBooking.Infrastructure.Persistence;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Infrastructure.Jobs;

public class NoShowJob
{
    private readonly RestaurantBookingDbContext _context;
    private readonly IRabbitMQService _rabbitService;

    public NoShowJob(RestaurantBookingDbContext context, IRabbitMQService rabbitService)
    {
        _context = context;
        _rabbitService = rabbitService;
    }

    public async Task ProcessNoShows()
    {
        var toleranceTime = DateTime.UtcNow.AddMinutes(-30);

        var lateReservations = await _context.Reservations
            .Where(r => r.Status == ReservationStatus.Confirmed && 
                        r.ReservationDate < toleranceTime)
            .ToListAsync();

        if (!lateReservations.Any()) return;

        foreach (var reservation in lateReservations)
        {
            reservation.MarkAsNoShow();
            _rabbitService.Publish(new { ReservationId = reservation.Id, Reason = "No-Show Automatico" }, "no-show-queue");
        }

        await _context.SaveChangesAsync();
        Console.WriteLine($"[Hangfire] Processados {lateReservations.Count} No-Shows.");
    }
}
