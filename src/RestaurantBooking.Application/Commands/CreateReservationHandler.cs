using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Entities;

namespace RestaurantBooking.Application.Commands;

public class CreateReservationHandler
{
    private readonly IReservationRepository _repository;

    public CreateReservationHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationDto> Handle(CreateReservationRequest request)
    {
        // Cria a Entidade
        var reservation = Reservation.Create(
            request.CustomerName,
            request.CustomerEmail,
            request.CustomerPhone,
            request.ReservationDate,
            request.NumberOfGuests,
            request.RestaurantId
        );

        // Persiste
        await _repository.AddAsync(reservation);

        // Retorna DTO
        return new ReservationDto(
            reservation.Id,
            reservation.Code,
            reservation.CustomerName,
            reservation.ReservationDate,
            reservation.NumberOfGuests,
            reservation.Status.ToString()
        );
    }
}
