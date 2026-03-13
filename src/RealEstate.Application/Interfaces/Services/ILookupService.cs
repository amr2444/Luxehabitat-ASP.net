using RealEstate.Application.DTOs.Lookup;

namespace RealEstate.Application.Interfaces.Services;

public interface ILookupService
{
    Task<IReadOnlyCollection<LocationOptionDto>> GetCitiesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LocationOptionDto>> GetDistrictsAsync(Guid? cityId, CancellationToken cancellationToken = default);
}
