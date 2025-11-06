using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProducts.Domain;
public class ProductEntityTests
{
    [Fact]
    public void Constructor_Should_Set_Properties()
    {
        // Arrange
        var name = "Produto A";
        var category = "Categoria X";
        var cost = 12.34m;

        // Act
        var p = new Product(name, category, cost);

        // Assert
        Assert.NotEqual(Guid.Empty, p.Id);
        Assert.Equal(name, p.Name);
        Assert.Equal(category, p.Category);
        Assert.Equal(cost, p.UnitCost);
        Assert.True(p.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Update_Should_Change_Properties()
    {
        // Arrange
        var p = new Product("Sapato", "Vesturario", 1.0m);

        // Act
        p.Update("Tenis", "Calcados", 2.5m);

        // Assert
        Assert.Equal("Tenis", p.Name);
        Assert.Equal("Calcados", p.Category);
        Assert.Equal(2.5m, p.UnitCost);
    }
}
