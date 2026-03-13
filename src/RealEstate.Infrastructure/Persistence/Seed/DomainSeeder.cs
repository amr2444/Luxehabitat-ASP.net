using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Seed;

public sealed class DomainSeeder(
    ApplicationDbContext context,
    ILogger<DomainSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seed des villes et quartiers de base.");

        var citySeeds = new (string Name, string Slug, string PostalCode)[]
        {
            ("Paris", "paris", "75000"),
            ("Lyon", "lyon", "69000"),
            ("Marseille", "marseille", "13000"),
            ("Bordeaux", "bordeaux", "33000"),
            ("Lille", "lille", "59000"),
            ("Toulouse", "toulouse", "31000"),
            ("Nice", "nice", "06000"),
            ("Nantes", "nantes", "44000"),
            ("Strasbourg", "strasbourg", "67000"),
            ("Montpellier", "montpellier", "34000"),
            ("Rennes", "rennes", "35000"),
            ("Grenoble", "grenoble", "38000"),
            ("Toulon", "toulon", "83000"),
            ("Reims", "reims", "51100"),
            ("Le Havre", "le-havre", "76600"),
            ("Saint-Etienne", "saint-etienne", "42000"),
            ("Dijon", "dijon", "21000"),
            ("Angers", "angers", "49000"),
            ("Nimes", "nimes", "30000"),
            ("Aix-en-Provence", "aix-en-provence", "13100"),
            ("Annecy", "annecy", "74000"),
            ("Cannes", "cannes", "06400"),
            ("Antibes", "antibes", "06600"),
            ("La Rochelle", "la-rochelle", "17000"),
            ("Biarritz", "biarritz", "64200"),
            ("Clermont-Ferrand", "clermont-ferrand", "63000")
        };

        var existingCities = await context.Cities.ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);
        var createdCities = new List<City>();

        foreach (var citySeed in citySeeds)
        {
            if (existingCities.ContainsKey(citySeed.Name))
            {
                continue;
            }

            var city = new City
            {
                Name = citySeed.Name,
                Slug = citySeed.Slug,
                PostalCode = citySeed.PostalCode
            };

            createdCities.Add(city);
            existingCities[citySeed.Name] = city;
        }

        if (createdCities.Count > 0)
        {
            await context.Cities.AddRangeAsync(createdCities, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var districtSeeds = new (string City, string Name, string Slug)[]
        {
            ("Paris", "Le Marais", "le-marais"),
            ("Paris", "Saint-Germain-des-Pres", "saint-germain-des-pres"),
            ("Lyon", "La Presqu'ile", "la-presquile"),
            ("Lyon", "Croix-Rousse", "croix-rousse"),
            ("Marseille", "Le Panier", "le-panier"),
            ("Marseille", "Prado", "prado"),
            ("Bordeaux", "Chartrons", "chartrons"),
            ("Lille", "Vieux-Lille", "vieux-lille"),
            ("Toulouse", "Carmes", "carmes"),
            ("Nice", "Cimiez", "cimiez"),
            ("Nantes", "Ile de Nantes", "ile-de-nantes"),
            ("Strasbourg", "Petite France", "petite-france"),
            ("Montpellier", "Ecusson", "ecusson"),
            ("Rennes", "Thabor", "thabor")
        };

        var existingDistricts = await context.Districts
            .Include(x => x.City)
            .ToDictionaryAsync(x => $"{x.City.Name}|{x.Name}", StringComparer.OrdinalIgnoreCase, cancellationToken);

        var createdDistricts = new List<District>();

        foreach (var districtSeed in districtSeeds)
        {
            if (!existingCities.TryGetValue(districtSeed.City, out var city))
            {
                continue;
            }

            var key = $"{districtSeed.City}|{districtSeed.Name}";
            if (existingDistricts.ContainsKey(key))
            {
                continue;
            }

            createdDistricts.Add(new District
            {
                CityId = city.Id,
                Name = districtSeed.Name,
                Slug = districtSeed.Slug
            });
        }

        if (createdDistricts.Count > 0)
        {
            await context.Districts.AddRangeAsync(createdDistricts, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
