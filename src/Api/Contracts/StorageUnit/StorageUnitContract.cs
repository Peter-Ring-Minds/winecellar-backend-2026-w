using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.StorageUnit;

public class StorageUnitContract : BaseContract
{
        public Guid StorageUnitId { get; set; }
        public Guid CellarId { get; set; }
}