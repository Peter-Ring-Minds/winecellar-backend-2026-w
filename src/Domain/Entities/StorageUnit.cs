namespace Domain.Entities;

public class StorageUnit
{
    public Guid Id { get; set; }

    public Guid CellarId { get; set; }
    public Cellar? Cellar { get; set; }

    public required string Name { get; set; }

    public ICollection<Wine> Wines { get; set; } = new List<Wine>();
}
