namespace RealEstate.Tests.Architecture;

public sealed class SolutionStructureTests
{
    [Fact]
    public void DomainAssemblyMarker_ShouldBeAccessible()
    {
        var markerType = typeof(RealEstate.Domain.Entities.EntityAssemblyMarker);

        Assert.NotNull(markerType);
    }
}
