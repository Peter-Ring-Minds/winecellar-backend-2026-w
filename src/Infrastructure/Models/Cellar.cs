namespace Domain;

public class Cellar
{
    public Guid CellarId { get; set; }

    // Foreign key to user
    public Guid UserId { get; set; }
    public List<StorageUnit> StorageUnits { get; set; } = new();
}
