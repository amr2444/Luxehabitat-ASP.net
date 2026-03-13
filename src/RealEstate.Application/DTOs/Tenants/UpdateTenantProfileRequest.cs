namespace RealEstate.Application.DTOs.Tenants;

public sealed record UpdateTenantProfileRequest(
    string? Occupation,
    decimal? MonthlyIncome,
    Guid? PreferredCityId,
    decimal? PreferredBudgetMin,
    decimal? PreferredBudgetMax);
