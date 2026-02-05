namespace Domain;

public class Wine
{
    public Guid WineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Wineyard { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Vintage { get; set; }

}