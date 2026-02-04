namespace Domain.Entities;

public class CellarMembership
{
    public Guid CellarId { get; set; }
    public Cellar? Cellar { get; set; }

    public Guid UserId { get; set; }

    public CellarRole Role { get; set; }
}
