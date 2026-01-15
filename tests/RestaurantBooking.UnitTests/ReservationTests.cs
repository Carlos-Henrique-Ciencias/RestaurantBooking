using FluentAssertions;
using RestaurantBooking.Domain.Entities;
using RestaurantBooking.Domain.Exceptions;
using RestaurantBooking.Domain.ValueObjects;
using Xunit;

namespace RestaurantBooking.UnitTests;

public class ReservationTests
{
    [Fact] // Teste 1 - Cria uma reserva com sucesso 
    public void Create_WithValidData_ShouldReturnReservation()
    {
        // Arrange & Act
        var reservation = Reservation.Create("Carlos", "carlos@email.com", "11999999999", DateTime.UtcNow.AddDays(1), 2, 1);

        // Assert
        reservation.Should().NotBeNull();
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.Code.Should().NotBeEmpty();
    }

    [Fact] // Teste 2 - Impede criação com 0 convidados (Validação)
    public void Create_WithZeroGuests_ShouldThrowException()
    {
        // Act
        Action act = () => Reservation.Create("Carlos", "email", "phone", DateTime.UtcNow, 0, 1);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("O número de convidados deve ser maior que zero.");
    }

    [Fact] // Teste 3 - Confirma uma reserva pendente (Mudança de Estado)
    public void Confirm_PendingReservation_ShouldChangeToConfirmed()
    {
        // Arrange
        var reservation = Reservation.Create("Carlos", "email", "phone", DateTime.UtcNow, 2, 1);

        // Act
        reservation.Confirm();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact] // Teste 4 - Impede confirmar algo que já está confirmado (Regra de Negócio)
    public void Confirm_AlreadyConfirmed_ShouldThrowException()
    {
        // Arrange
        var reservation = Reservation.Create("Carlos", "email", "phone", DateTime.UtcNow, 2, 1);
        reservation.Confirm();

        // Act
        Action act = () => reservation.Confirm();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Apenas reservas pendentes podem ser confirmadas.");
    }

    [Fact] // Teste 5 - Realiza Check-in numa reserva confirmada
    public void CheckIn_ConfirmedReservation_ShouldChangeToCheckedIn()
    {
        // Arrange
        var reservation = Reservation.Create("Carlos", "email", "phone", DateTime.UtcNow, 2, 1);
        reservation.Confirm();

        // Act
        reservation.CheckIn();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.CheckedIn);
        reservation.CheckedInAt.Should().NotBeNull();
    }

    [Fact] // Teste 6 - Impede Check-in se a reserva ainda estiver pendente
    public void CheckIn_PendingReservation_ShouldThrowException()
    {
        // Arrange
        var reservation = Reservation.Create("Carlos", "email", "phone", DateTime.UtcNow, 2, 1);

        // Act
        Action act = () => reservation.CheckIn();

        // Assert
        act.Should().Throw<DomainException>().WithMessage("O check-in só pode ser feito em reservas confirmadas.");
    }
}