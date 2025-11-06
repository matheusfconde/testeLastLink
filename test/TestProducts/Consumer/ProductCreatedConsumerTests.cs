using Domain.Entities;
using Domain.Events;
using Infrastructure.DataBase;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace TestProducts.Consumer;

/// <summary>
/// Conjunto de testes para a lógica central do consumidor ProductCreatedConsumer.
/// Testa se a mensagem recebida é processada corretamente e a entidade ProductEvent é persistida.
/// </summary>
public class ProductCreatedConsumerTests : IDisposable
{
    // Usamos o AppDbContext real (instanciado com In-Memory) em vez de mocká-lo.
    private readonly AppDbContext _context;
    private readonly ProductCreatedConsumer _consumer;

    public ProductCreatedConsumerTests()
    {
        // 1. Configura as opções para usar um banco de dados In-Memory exclusivo
        // Isso resolve o erro de 'Non-overridable members' ao não usar Moq no DbContext.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // 2. Inicializa o contexto real e o consumidor
        _context = new AppDbContext(options);
        // O consumidor agora recebe um DbContext funcional
        _consumer = new ProductCreatedConsumer(_context);
    }

    [Fact]
    public async Task Consume_Should_CreateAndSaveProductEvent()
    {
        // Arrange
        // Dados de um evento de produto simulado
        var productEvent = new ProductCreatedEvent
        {
            Id = Guid.NewGuid(),
            Name = "Laptop Gamer",
            Category = "Eletronics",
            UnitCost = 5500.00m,
            CreatedAt = DateTime.UtcNow
        };

        // Simula o contexto de consumo do MassTransit
        var mockContext = new Mock<ConsumeContext<ProductCreatedEvent>>();
        mockContext.Setup(c => c.Message).Returns(productEvent);

        // Act
        await _consumer.Consume(mockContext.Object);

        // Assert 1: Verifica se a entidade foi salva no banco de dados
        // Query no contexto real (In-Memory) para verificar a persistência
        var savedEvent = _context.ProductEvents.FirstOrDefault();

        Assert.NotNull(savedEvent);
        Assert.Equal(1, _context.ProductEvents.Count());
        Assert.Equal("product.created", savedEvent.EventType);

        // Assert 2: Verifica se o Payload (JSON) da entidade salva contém os dados originais
        var deserializedPayload = JsonConvert.DeserializeObject<ProductCreatedEvent>(savedEvent.Payload);
        Assert.Equal(productEvent.Id, deserializedPayload.Id);
        Assert.Equal(productEvent.Name, deserializedPayload.Name);
        Assert.Equal(productEvent.UnitCost, deserializedPayload.UnitCost);
    }

    /// <summary>
    /// Garante que o banco de dados In-Memory é limpo após cada teste.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}