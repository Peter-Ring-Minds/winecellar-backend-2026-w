namespace Domain;

public class Wine : BaseModel
{
    public string Wineyard { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Vintage { get; set; }

    // Foreign key to StorageUnit
    public Guid? StorageUnitId { get; set; }
    public StorageUnit? StorageUnit { get; set; }

}