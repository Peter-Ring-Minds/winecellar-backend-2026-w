namespace Domain.Entities;

public class Cellar
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public ICollection<StorageUnit> StorageUnits { get; set; } = new List<StorageUnit>();
    public ICollection<CellarMembership> Memberships { get; set; } = new List<CellarMembership>();
}
