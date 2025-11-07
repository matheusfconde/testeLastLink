using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TestProducts.Infrastructure.DataBase;

public class ProductRepositoryTests
{
    private static AppDbContext CreateInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Product()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(AddAsync_Should_Add_Product));
        var repo = new ProductRepository(ctx);

        var product = new Product("Produto Teste", "Categoria X", 99.90m);

        // Act
        var added = await repo.AddAsync(product);

        // Assert
        Assert.NotNull(added);
        Assert.Single(ctx.Products);
        Assert.Equal("Produto Teste", ctx.Products.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_When_Exists()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(GetByIdAsync_Should_Return_Product_When_Exists));
        var product = new Product("Produto A", "Categoria A", 10m);
        ctx.Products.Add(product);
        await ctx.SaveChangesAsync();

        var repo = new ProductRepository(ctx);

        // Act
        var result = await repo.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result?.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_NotExists()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(GetByIdAsync_Should_Return_Null_When_NotExists));
        var repo = new ProductRepository(ctx);

        // Act
        var result = await repo.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Products()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(GetAllAsync_Should_Return_All_Products));
        ctx.Products.Add(new Product("P1", "C1", 1));
        ctx.Products.Add(new Product("P2", "C2", 2));
        await ctx.SaveChangesAsync();

        var repo = new ProductRepository(ctx);

        // Act
        var all = (await repo.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, all.Count);
        Assert.Contains(all, p => p.Name == "P1");
    }

    [Fact]
    public async Task UpdateAsync_Should_Modify_Existing_Product()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(UpdateAsync_Should_Modify_Existing_Product));
        var product = new Product("Old", "Cat", 10);
        ctx.Products.Add(product);
        await ctx.SaveChangesAsync();

        var repo = new ProductRepository(ctx);

        // Act
        product.Update("New", "NewCat", 25);
        await repo.UpdateAsync(product);

        // Assert
        var updated = await ctx.Products.FindAsync(product.Id);
        Assert.Equal("New", updated!.Name);
        Assert.Equal("NewCat", updated.Category);
        Assert.Equal(25, updated.UnitCost);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Product()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(DeleteAsync_Should_Remove_Product));
        var product = new Product("Delete", "Cat", 50);
        ctx.Products.Add(product);
        await ctx.SaveChangesAsync();

        var repo = new ProductRepository(ctx);

        // Act
        await repo.DeleteAsync(product.Id);

        // Assert
        var deleted = await ctx.Products.FindAsync(product.Id);
        Assert.Null(deleted);
        Assert.Empty(ctx.Products);
    }

    [Fact]
    public async Task DeleteAsync_Should_Not_Throw_When_Id_NotFound()
    {
        // Arrange
        var ctx = CreateInMemoryDbContext(nameof(DeleteAsync_Should_Not_Throw_When_Id_NotFound));
        var repo = new ProductRepository(ctx);

        // Act & Assert
        var ex = await Record.ExceptionAsync(() => repo.DeleteAsync(Guid.NewGuid()));
        Assert.Null(ex);
    }
}