namespace Domain.Events;

public class ProductCreatedEvent
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Category { get; init; } = null!;
    public decimal UnitCost { get; init; }
    public DateTime CreatedAt { get; init; }
}
