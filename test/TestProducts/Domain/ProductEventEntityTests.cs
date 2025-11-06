using Domain.Entities;
using System;
using Xunit;

namespace TestProducts.Domain;

public class ProductEventTests
{
    [Fact]
    public void Should_Assign_All_Properties_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var eventType = "product.created";
        var payload = "{\"id\":\"123\",\"name\":\"Produto Teste\"}";
        var createdAt = new DateTime(2025, 11, 6, 10, 0, 0, DateTimeKind.Utc);

        // Act
        var evt = new ProductEvent
        {
            Id = id,
            EventType = eventType,
            Payload = payload,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(id, evt.Id);
        Assert.Equal(eventType, evt.EventType);
        Assert.Equal(payload, evt.Payload);
        Assert.Equal(createdAt, evt.CreatedAt);

        Assert.NotNull(evt.EventType);
        Assert.NotNull(evt.Payload);
    }

    [Fact]
    public void Default_Constructor_Should_Create_Valid_Object()
    {
        // Act
        var evt = new ProductEvent();

        // Assert
        Assert.NotNull(evt); 
        Assert.Null(evt.EventType); 
        Assert.Null(evt.Payload);
        Assert.Equal(default(Guid), evt.Id);
        Assert.Equal(default(DateTime), evt.CreatedAt);
    }

    [Fact]
    public void Two_Instances_With_Same_Data_Should_NotBeSameReference()
    {
        // Arrange
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var eventType = "product.created";
        var payload = "{\"key\":\"value\"}";

        var evt1 = new ProductEvent
        {
            Id = id,
            EventType = eventType,
            Payload = payload,
            CreatedAt = now
        };

        var evt2 = new ProductEvent
        {
            Id = id,
            EventType = eventType,
            Payload = payload,
            CreatedAt = now
        };

        // Act & Assert
        Assert.NotSame(evt1, evt2);
        Assert.Equal(evt1.Id, evt2.Id);
        Assert.Equal(evt1.EventType, evt2.EventType);
        Assert.Equal(evt1.Payload, evt2.Payload);
        Assert.Equal(evt1.CreatedAt, evt2.CreatedAt);
    }

    [Fact]
    public void Should_Allow_Update_Of_Properties()
    {
        // Arrange
        var evt = new ProductEvent
        {
            Id = Guid.NewGuid(),
            EventType = "product.created",
            Payload = "{}",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        evt.EventType = "product.updated";
        evt.Payload = "{\"updated\":true}";
        evt.CreatedAt = evt.CreatedAt.AddMinutes(10);

        // Assert
        Assert.Equal("product.updated", evt.EventType);
        Assert.Contains("updated", evt.Payload);
    }
}