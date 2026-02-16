namespace Domain;

public class Wine : BaseModel
{
    public string Wineyard { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Vintage { get; set; }
    public int Quantity { get; set; } = 1;

    // Foreign key to StorageUnit
    public Guid? StorageUnitId { get; set; }
    public StorageUnit? StorageUnit { get; set; }

}