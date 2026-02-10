using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Wine;

public record RegisterWineRequest([Required]Guid StorageUnitId, string Name, string Wineyard, string Type, int Vintage);