namespace Api.Contracts.Cellars;

public sealed record CellarSummary(Guid Id, int StorageUnitCount)
{
    public CellarSummary(Domain.Cellar cellar) : this(cellar.CellarId, cellar.StorageUnits.Count) {}
};