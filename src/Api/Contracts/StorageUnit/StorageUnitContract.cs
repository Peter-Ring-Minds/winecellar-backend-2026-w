using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.StorageUnit;

public record StorageUnitContract (Guid StorageUnitId, [Required] Guid CellarId, string Name);