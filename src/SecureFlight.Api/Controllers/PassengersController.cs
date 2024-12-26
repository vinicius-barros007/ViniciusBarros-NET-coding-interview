using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SecureFlight.Api.Models;
using SecureFlight.Api.Utils;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PassengersController(
    IService<Passenger> personService,
    IService<Flight> flightService,
    IRepository<Flight> flightRepository,
    IMapper mapper)
    : SecureFlightBaseController(mapper)
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> Get()
    {
        var passengers = await personService.GetAllAsync();
        return MapResultToDataTransferObject<IReadOnlyList<Passenger>, IReadOnlyList<PassengerDataTransferObject>>(passengers);
    }
    
    [HttpGet("/flights/{flightId:long}/passengers")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> GetPassengersByFlight(long flightId)
    {
        var passengers = await personService.FilterAsync(p => p.Flights.Any(x => x.Id == flightId));
        return !passengers.Succeeded ?
            NotFound($"No passengers were found for the flight {flightId}") :
            MapResultToDataTransferObject<IReadOnlyList<Passenger>, IReadOnlyList<PassengerDataTransferObject>>(passengers);
    }

    //Add the possibility to add an existing passenger to a flight.
    [HttpPost("/flights/{flightId:long}/passenger")]
    [ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    public async Task<IActionResult> AddPassengersByFlight(long flightId, [FromQuery]string passengerId)
    {
        var dbPassenger = await personService.FilterAsync(x => x.Id == passengerId);
        if (!dbPassenger.Result.Any())
        {
            return NotFound($"Passenger {passengerId} was not found.");
        }

        //TODO: Check if the passenger already exists for this flight

        var dbFlight = await flightService.FilterAsync(x => x.Id == flightId);
        if (dbFlight.Result is null)
        {
            return NotFound($"Flight {flightId} was not found.");
        }

        var flight = dbFlight.Result.First();
        flight.Passengers.Add(dbPassenger.Result.First());

        flightRepository.Update(flight);
        await flightRepository.SaveChangesAsync();  

        return Ok();    
    }

    ////Add the possibility to remove a passenger from a flight. If the passenger has no other
    ////pending flights, then remove passenger from the system entirely.
    //[HttpDelete("/flights/{flightId:long}/passenger")]
    //[ProducesResponseType(typeof(IEnumerable<PassengerDataTransferObject>), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseActionResult))]
    //public async Task<IActionResult> DeletePassengersByFlight(long flightId, [FromQuery] string passengerId)
    //{
    //    var dbPassenger = await personService.FilterAsync(x => x.Id == passengerId);
    //    if (dbPassenger.Result is null)
    //    {
    //        return NotFound($"Passenger {passengerId} was not found.");
    //    }

    //    var dbFlight = await flightService.FilterAsync(x => x.Id == flightId);
    //    if (dbFlight.Result is null)
    //    {
    //        return NotFound($"Flight {flightId} was not found.");
    //    }

    //    var flight = dbFlight.Result.First();




    //    flight.Passengers.Add(dbPassenger.Result.First());

    //    flightRepository.Update(flight);
    //    await flightRepository.SaveChangesAsync();

    //    return Ok();
    //}
}