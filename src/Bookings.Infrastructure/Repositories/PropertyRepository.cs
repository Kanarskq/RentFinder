using Bookings.Domain.AggregatesModel.PropertyAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly BookingContext _context;

    public PropertyRepository(BookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public Property Add(Property property)
    {
        return _context.Properties.Add(property).Entity;
    }

    public async Task<Property?> GetAsync(int propertyId)
    {
        return await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == propertyId);
    }

    public async Task<Property?> GetByOwnerAsync(int ownerId)
    {
        return await _context.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OwnerId == ownerId);
    }

    public async Task<IEnumerable<Property>> GetAllAsync()
    {
        return await _context.Properties
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByStatusAsync(string status)
    {
        return await _context.Properties
            .AsNoTracking()
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public void Update(Property property)
    {
        _context.Properties.Update(property);
    }
}
