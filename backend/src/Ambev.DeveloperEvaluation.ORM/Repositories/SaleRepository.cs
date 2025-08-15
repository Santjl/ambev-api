using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleRepository(DefaultContext context) : ISaleRepository
    {
        public async Task<Sale> CreateAsync(Sale sale, CancellationToken ct)
        {
            await context.Sales.AddAsync(sale, ct);
            await context.SaveChangesAsync(ct);
            return sale;
        }

        public Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return context.Sales
                .Include(x => x.Items)
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public Task<Sale?> GetByNumberAsync(string number, CancellationToken ct)
        {
            return context.Sales
                .FirstOrDefaultAsync(s => s.Number == number, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await context.SaveChangesAsync(ct);
        } 
    }
}
