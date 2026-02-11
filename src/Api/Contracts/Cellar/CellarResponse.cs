namespace Api.Contracts.Cellar;
public class CellarResponse : BaseContract
{
    public Guid CellarId { get; set; }
    public Guid UserId { get; set; }
}