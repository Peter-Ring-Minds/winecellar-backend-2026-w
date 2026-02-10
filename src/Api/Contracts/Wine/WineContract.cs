namespace Api.Contracts.Wine;

public record WineContract(Guid WineId, Guid StorageUnitId, string StorageUnitName, string Name, string Wineyard, string Type, int Vintage);