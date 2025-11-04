using Application.Interfaces;
using Domain.Entities;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _ctx;
    public ProductRepository(AppDbContext ctx) => _ctx = ctx;
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var p = await _ctx.Products.FindAsync(new object[] { id }, cancellationToken);
        if (p != null)
        {
            _ctx.Products.Remove(p);
            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _ctx.Products.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _ctx.Products.Update(product);
        await _ctx.SaveChangesAsync(cancellationToken);
    }
}
