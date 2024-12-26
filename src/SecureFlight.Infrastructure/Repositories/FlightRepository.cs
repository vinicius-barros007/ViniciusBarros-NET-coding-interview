using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecureFlight.Core.Entities;
using SecureFlight.Core.Interfaces;

namespace SecureFlight.Infrastructure.Repositories;


public interface IFlightRepository : IRepository<Flight>
{
}

public class FlightRepository(SecureFlightDbContext context) : IFlightRepository
{
    public async Task<IReadOnlyList<Flight>> FilterAsync(Expression<Func<Flight, bool>> predicate)
    {
        return await context.Flights
            .Include(f => f.Passengers)
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Flight>> GetAllAsync()
    {
        return await context.Flights
            .Include(f => f.Passengers)
            .AsNoTracking()
            .ToListAsync();

    }

    public async Task<Flight> GetByIdAsync(params object[] keyValues)
    {
        return await context.Flights.FindAsync(keyValues);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public Flight Update(Flight entity)
    {
        var entry = context.Entry(entity);
        entry.State = EntityState.Modified;
        return entity;
    }
}