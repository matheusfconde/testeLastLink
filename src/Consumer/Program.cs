using Domain.Events;
using Infrastructure.DataBase;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        var conn = ctx.Configuration.GetConnectionString("Default") ?? ctx.Configuration["ConnectionStrings__Default"];
        //var rabbitHost = ctx.Configuration["RabbitMq__Host"] ?? "localhost";
        var rabbitHost = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost";

        services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(conn));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProductCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("product-created-queue", e =>
                {
                    e.ConfigureConsumer<ProductCreatedConsumer>(context);
                });
            });
        });
    });

var host = builder.Build();
await host.RunAsync();

// Consumer implementation
public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly AppDbContext _db;
    public ProductCreatedConsumer(AppDbContext db) => _db = db;

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var evt = context.Message;
        var payload = Newtonsoft.Json.JsonConvert.SerializeObject(evt);

        var productEvent = new ProductEvent
        {
            Id = Guid.NewGuid(),
            EventType = "product.created",
            Payload = payload,
            CreatedAt = DateTime.UtcNow
        };

        _db.ProductEvents.Add(productEvent);
        await _db.SaveChangesAsync();
    }
}
