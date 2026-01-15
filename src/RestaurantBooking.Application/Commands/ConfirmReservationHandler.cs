using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Exceptions;

namespace RestaurantBooking.Application.Commands;

public record ConfirmReservationRequest(int Id);

public class ConfirmReservationHandler
{
    private readonly IReservationRepository _repository;

    public ConfirmReservationHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(ConfirmReservationRequest request)
    {
        var reservation = await _repository.GetByIdAsync(request.Id);
        if (reservation == null) throw new DomainException("Reserva não encontrada.");

        reservation.Confirm(); // Regra de Domínio

        await _repository.UpdateAsync(reservation);
    }
}
