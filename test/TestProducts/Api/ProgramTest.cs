using Application.Interfaces;
using Domain.Entities;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestProducts.Api;

/// <summary>
/// Conjunto de testes para a implementação do ProductRepository, utilizando um banco de dados in-memory
/// para garantir o isolamento e a rapidez dos testes.
/// </summary>
public class ProductRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _repository;

    public ProductRepositoryTests()
    {
        // 1. Configura o DbContextOptions para usar um banco de dados In-Memory exclusivo
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Garante um DB isolado para cada teste
            .Options;

        // 2. Inicializa o contexto e o repositório
        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        var newProduct = new Product("Smartphone X", "Eletronics", 1200.50m);

        // Act
        await _repository.AddAsync(newProduct);
        await _context.SaveChangesAsync();

        // Assert
        var retrievedProduct = await _context.Products.FindAsync(newProduct.Id);
        Assert.NotNull(retrievedProduct);
        Assert.Equal("Smartphone X", retrievedProduct.Name);
        Assert.Equal(1200.50m, retrievedProduct.UnitCost);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductIfExists()
    {
        // Arrange
        var existingProduct = new Product("Monitor 24p", "IT Gear", 850.00m);
        _context.Products.Add(existingProduct);
        await _context.SaveChangesAsync();
        // Detaches the entity to simulate a fresh query
        _context.Entry(existingProduct).State = EntityState.Detached;

        // Act
        var result = await _repository.GetByIdAsync(existingProduct.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingProduct.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyProductInDatabase()
    {
        // Arrange
        var productToUpdate = new Product("Old Name", "Old Cat", 100.00m);
        _context.Products.Add(productToUpdate);
        await _context.SaveChangesAsync();
        _context.Entry(productToUpdate).State = EntityState.Detached;

        // Recupera o produto para simular o fluxo real de update
        var retrievedProduct = await _repository.GetByIdAsync(productToUpdate.Id);
        retrievedProduct.Update("New Name", "New Cat", 150.00m);

        // Act
        await _repository.UpdateAsync(retrievedProduct);

        // Assert
        var updatedProduct = await _context.Products.FindAsync(retrievedProduct.Id);
        Assert.Equal("New Name", updatedProduct.Name);
        Assert.Equal("New Cat", updatedProduct.Category);
        Assert.Equal(150.00m, updatedProduct.UnitCost);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProductFromDatabase()
    {
        // Arrange
        var productToDelete = new Product("Widget A", "Tools", 50.00m);
        _context.Products.Add(productToDelete);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(productToDelete.Id);

        // Assert
        var deletedProduct = await _context.Products.FindAsync(productToDelete.Id);
        Assert.Null(deletedProduct);
    }
}