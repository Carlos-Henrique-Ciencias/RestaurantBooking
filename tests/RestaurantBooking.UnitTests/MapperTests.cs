using FluentAssertions;
using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Domain.Entities;
using Xunit;

namespace RestaurantBooking.UnitTests;

public class MapperTests
{
    [Fact] // Teste 11 - Mapeia corretamente os dados da Entidade para o DTO
    public void ReservationDto_ShouldMapCorrectly()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var r = Reservation.Create("Carlos", "email", "phone", date, 4, 1);
        
        // Act
        var dto = new ReservationDto(r.Id, r.Code, r.CustomerName, r.ReservationDate, r.NumberOfGuests, r.Status.ToString());

        // Assert
        dto.CustomerName.Should().Be("Carlos");
        dto.NumberOfGuests.Should().Be(4);
        dto.Status.Should().Be("Pending");
    }

    [Fact] // Teste 12 - Calcula corretamente o total de páginas (Matemática)
    public void PagedResult_CalculatesTotalPagesCorrectly()
    {
        // Arrange
        var result = new PagedResult<string> { TotalCount = 100, PageSize = 10 };

        // Assert
        result.TotalPages.Should().Be(10);
    }
}