namespace Api.Contracts.Wine;


public class WineContract : BaseContract
{
    public Guid StorageUnitId { get; set; }
    public string Wineyard { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Vintage { get; set; }
    public int Quantity { get; set; } = 1;
}