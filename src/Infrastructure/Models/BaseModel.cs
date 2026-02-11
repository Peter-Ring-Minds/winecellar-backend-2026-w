namespace Domain;
public class BaseModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Name { get; set; } = string.Empty;

    public Guid UserId { get; set; }
}