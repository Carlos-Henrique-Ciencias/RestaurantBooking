using FluentAssertions;
using Moq;
using RestaurantBooking.Application.Commands;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Domain.Entities;
using Xunit;

namespace RestaurantBooking.UnitTests;

public class HandlerTests
{
    private readonly Mock<IReservationRepository> _repoMock;
    
   
    public HandlerTests()
    {
        _repoMock = new Mock<IReservationRepository>();
    }

    [Fact]
    public async Task CreateHandler_ValidCommand_ShouldCallRepository()
    {
        // Arrange
        
        var handler = new CreateReservationHandler(_repoMock.Object);
        var command = new CreateReservationRequest 
        { 
            CustomerName = "Teste", 
            CustomerEmail = "t@t.com", 
            CustomerPhone = "11999999999", 
            ReservationDate = DateTime.UtcNow.AddDays(1), 
            NumberOfGuests = 2, 
            RestaurantId = 1 
        };

        // Act
        var result = await handler.Handle(command);

        // Assert
        result.Should().NotBeNull();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmHandler_ValidId_ShouldUpdateRepository()
    {
        // Arrange
        var reservation = Reservation.Create("Teste", "email", "phone", DateTime.UtcNow.AddDays(1), 2, 1);
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        
        var handler = new ConfirmReservationHandler(_repoMock.Object);

        // Act
        await handler.Handle(new ConfirmReservationRequest(1));

        // Assert
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task CheckInHandler_ValidId_ShouldUpdateRepository()
    {
        // Arrange
        var reservation = Reservation.Create("Teste", "email", "phone", DateTime.UtcNow.AddDays(1), 2, 1);
        reservation.Confirm();
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
        
        var handler = new CheckInReservationHandler(_repoMock.Object);

        // Act
        await handler.Handle(new CheckInReservationRequest(1));

        // Assert
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Once);
    }
}
