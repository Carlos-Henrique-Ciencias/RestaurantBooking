using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Exceptions;

namespace RestaurantBooking.Application.Commands;

public record CheckInReservationRequest(int Id);

public class CheckInReservationHandler
{
    private readonly IReservationRepository _repository;

    public CheckInReservationHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CheckInReservationRequest request)
    {
        var reservation = await _repository.GetByIdAsync(request.Id);
        if (reservation == null) throw new DomainException("Reserva não encontrada.");

        reservation.CheckIn(); // Regra de Domínio

        await _repository.UpdateAsync(reservation);
    }
}
