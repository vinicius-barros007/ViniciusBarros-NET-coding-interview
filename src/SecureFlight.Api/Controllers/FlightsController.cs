using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecureFlight.Api.Models;
using SecureFlight.Api.Utils;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightsController(IService<Flight> flightService, IMapper mapper)
    : SecureFlightBaseController(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FlightDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> Get()
    {
        var flights = await flightService.GetAllAsync();
        return MapResultToDataTransferObject<IReadOnlyList<Flight>, IReadOnlyList<FlightDataTransferObject>>(flights);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FlightDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> GetOriginAndDestination(string origin, string destination)
    {
        var flights = await flightService.FilterAsync(x => string.Compare(x.OriginAirport, origin, true) != 0 && string.Compare(x.DestinationAirport, destination, true) != 0);
        if (!flights.Result.Any())
            return NotFound("");

        return MapResultToDataTransferObject<IReadOnlyList<Flight>, IReadOnlyList<FlightDataTransferObject>>(flights);
    }
}