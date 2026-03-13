namespace RealEstate.Infrastructure.Persistence.Seed;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public string AdminEmail { get; set; } = "admin@realestate.local";

    public string AdminPassword { get; set; } = "Admin1234!";

    public string AdminFirstName { get; set; } = "System";

    public string AdminLastName { get; set; } = "Administrator";
}
