using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Infrastructure.DataBase;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// configurations from env
var connStr = builder.Configuration.GetConnectionString("Default") ?? builder.Configuration["ConnectionStrings__Default"];
var rabbitHost = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost";

// Add services
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connStr));

builder.Services.AddScoped<IProductRepository, ProductRepository>();

// MassTransit config
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        //Em ambiente produtivo, todas as credenciais ficaram em um cofre (secret manager, azure key vault)
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username("guest"); 
            h.Password("guest");
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations at startup (simple approach)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Endpoints
app.MapPost("/products", async (CreateProductRequest req, IProductRepository repo, IPublishEndpoint publish, ILogger<Program> logger) =>
{
    var product = new Product(req.Name, req.Category, req.UnitCost);
    await repo.AddAsync(product);

    var evt = new ProductCreatedEvent
    {
        Id = product.Id,
        Name = product.Name,
        Category = product.Category,
        UnitCost = product.UnitCost,
        CreatedAt = product.CreatedAt
    };

    // Caso o rabbit mq esteja fora/falhe, o retorno continuara.
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
    var cancellationToken = cts.Token;
    try
    {
        await publish.Publish(evt, cancellationToken);
    }
    catch (Exception ex)
    {        
        logger.LogError(ex, $"Falha ao publicar o evento ProductCreatedEvent para o RabbitMQ (Produto ID: {product.Id}). A transação principal foi concluída.");

        //Para uma evolução, jogar os eventos que falharam em uma tabela (lost_messages), e consumí-las no consumer.               
    }

    return Results.Created($"/products/{product.Id}", new
    {
        product.Id,
        product.Name,
        product.Category,
        product.UnitCost,
        product.CreatedAt
    });
});

app.MapGet("/products", async (IProductRepository repo) =>
{
    var products = (await repo.GetAllAsync()).Select(p => new
    {
        p.Id,
        p.Name,
        p.Category,
        p.UnitCost,
        p.CreatedAt
    });
    return Results.Ok(products);
});

app.MapGet("/products/{id:guid}", async (Guid id, IProductRepository repo) =>
{
    var p = await repo.GetByIdAsync(id);
    return p is null ? Results.NotFound() : Results.Ok(new { p.Id, p.Name, p.Category, p.UnitCost, p.CreatedAt });
});

app.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest req, IProductRepository repo) =>
{
    var p = await repo.GetByIdAsync(id);
    if (p == null) return Results.NotFound();
    p.Update(req.Name, req.Category, req.UnitCost);
    await repo.UpdateAsync(p);
    return Results.NoContent();
});

app.MapDelete("/products/{id:guid}", async (Guid id, IProductRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();

record CreateProductRequest(string Name, string Category, decimal UnitCost);
record UpdateProductRequest(string Name, string Category, decimal UnitCost);
