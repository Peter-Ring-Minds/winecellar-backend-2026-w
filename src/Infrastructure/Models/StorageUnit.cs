namespace Domain;

public class StorageUnit
{
    public Guid StorageUnitId { get; set; }
    public string StorageUnitName { get; set; } = string.Empty;
    public List<Wine> Wines { get; set; } = new();  

    // Foregin key to cellar
    public Guid CellarId { get; set; }
    public Cellar? Cellar { get; set; }
}