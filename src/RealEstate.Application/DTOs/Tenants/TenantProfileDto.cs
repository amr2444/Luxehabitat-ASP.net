namespace RealEstate.Application.DTOs.Tenants;

public sealed record TenantProfileDto(
    Guid Id,
    string UserId,
    string? Occupation,
    decimal? MonthlyIncome,
    Guid? PreferredCityId,
    decimal? PreferredBudgetMin,
    decimal? PreferredBudgetMax);
