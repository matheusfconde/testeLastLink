namespace Application.DTOs;

public record ProductoDto(Guid id, string Name, string Category, decimal UnitCost, DateTime CreatedAt);
