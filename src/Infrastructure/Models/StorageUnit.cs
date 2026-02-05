namespace Domain;

public class StorageUnit
{
    public Guid StorageUnitId { get; set; }
    public string StorageName { get; set; } = string.Empty;
    public List<Wine> Wines { get; set; } = new();  
}