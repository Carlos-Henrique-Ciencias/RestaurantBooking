using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.Application.Commands;
using RestaurantBooking.Application.Queries;
using RestaurantBooking.Application.DTOs;

namespace RestaurantBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly CreateReservationHandler _createHandler;
    private readonly GetAllReservationsHandler _getAllHandler;
    private readonly ConfirmReservationHandler _confirmHandler;
    private readonly CheckInReservationHandler _checkInHandler;
    private readonly GetDashboardMetricsHandler _dashboardHandler;

    public ReservationsController(
        CreateReservationHandler createHandler,
        GetAllReservationsHandler getAllHandler,
        ConfirmReservationHandler confirmHandler,
        CheckInReservationHandler checkInHandler,
        GetDashboardMetricsHandler dashboardHandler)
    {
        _createHandler = createHandler;
        _getAllHandler = getAllHandler;
        _confirmHandler = confirmHandler;
        _checkInHandler = checkInHandler;
        _dashboardHandler = dashboardHandler;
    }

    [HttpGet("dashboard/{restaurantId}")]
    public async Task<IActionResult> GetDashboard([FromRoute] int restaurantId)
    {
        var result = await _dashboardHandler.Handle(new GetDashboardMetricsQuery { RestaurantId = restaurantId });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
    {
        try 
        {
            var response = await _createHandler.Handle(request);
            // CORREÇÃO AQUI: Mudamos de response.ReservationCode para response.Code
            return CreatedAtAction(nameof(Create), new { id = response.Code }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllReservationsQuery query)
    {
        var result = await _getAllHandler.Handle(query);
        return Ok(result);
    }

    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> Confirm([FromRoute] int id)
    {
        try { await _confirmHandler.Handle(new ConfirmReservationRequest(id)); return NoContent(); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id}/check-in")]
    public async Task<IActionResult> CheckIn([FromRoute] int id)
    {
        try { await _checkInHandler.Handle(new CheckInReservationRequest(id)); return NoContent(); }
        catch (Exception ex) { return BadRequest(new { error = ex.Message }); }
    }
}
