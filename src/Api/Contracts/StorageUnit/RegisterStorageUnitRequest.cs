namespace Api.Contracts.StorageUnit;
using System.ComponentModel.DataAnnotations;
public record RegisterStorageUnitRequest([Required] Guid CellarId, string Name);
