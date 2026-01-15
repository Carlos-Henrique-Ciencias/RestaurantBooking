using RestaurantBooking.Application.DTOs;
using RestaurantBooking.Application.Interfaces;

namespace RestaurantBooking.Application.Queries;

public class GetAllReservationsHandler
{
    private readonly IReservationRepository _repository;

    public GetAllReservationsHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ReservationDto>> Handle(GetAllReservationsQuery query)
    {
        // Chama o repositório
        var (items, totalCount) = await _repository.GetAllAsync(
            query.CustomerName,
            query.Statuses,
            query.DateStart,
            query.DateEnd,
            query.Page,
            query.PageSize,
            query.SortBy,
            query.SortDirection
        );

        // Mapeia Entidade -> DTO (Projeca os dados)
        var dtos = items.Select(r => new ReservationDto(
            r.Id,
            r.Code,
            r.CustomerName,
            r.ReservationDate,
            r.NumberOfGuests,
            r.Status.ToString()
        )).ToList();

        return new PagedResult<ReservationDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }
}
