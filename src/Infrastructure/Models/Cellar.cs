namespace Domain;

public class Cellar : BaseModel
{
    public List<StorageUnit> StorageUnits { get; set; } = new();
    
}
