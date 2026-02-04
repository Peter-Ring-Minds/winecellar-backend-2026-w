namespace Domain.Entities;

public class Wine
{
    public Guid Id { get; set; }

    public Guid StorageUnitId { get; set; }
    public StorageUnit? StorageUnit { get; set; }

    public required string Name { get; set; }
    public string? Producer { get; set; }
    public int? Vintage { get; set; }
    public int Quantity { get; set; }
}
