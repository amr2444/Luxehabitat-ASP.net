using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Lookup;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;

namespace RealEstate.Application.Services;

public sealed class LookupService(IApplicationDbContext context) : ILookupService
{
    public async Task<IReadOnlyCollection<LocationOptionDto>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Cities
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new LocationOptionDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LocationOptionDto>> GetDistrictsAsync(Guid? cityId, CancellationToken cancellationToken = default)
    {
        var query = context.Districts.AsNoTracking();
        if (cityId.HasValue)
        {
            query = query.Where(x => x.CityId == cityId.Value);
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new LocationOptionDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }
}
