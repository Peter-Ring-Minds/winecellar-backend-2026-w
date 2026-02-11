namespace Domain;

public class StorageUnit : BaseModel
{
    public List<Wine> Wines { get; set; } = new();  

    // Foregin key to cellar
    public Guid CellarId { get; set; }
    public Cellar? Cellar { get; set; }

}