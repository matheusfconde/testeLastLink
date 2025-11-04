namespace Domain.Entities;
public class Product
{
    public Guid Id  { get; private set; }
    public string Name { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public decimal UnitCost { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product() { }
    public Product(string name, string category, decimal unitCost)
    {
        Id = Guid.NewGuid();
        Name = name;
        Category = category;
        UnitCost = unitCost;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void Update (string name, string category, decimal unitCost)
    {
        Name = name;
        Category = category;
        UnitCost = unitCost;
    }
}
