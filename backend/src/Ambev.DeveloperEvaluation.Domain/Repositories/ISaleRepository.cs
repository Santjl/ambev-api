using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Sale> CreateAsync(Sale sale, CancellationToken ct);
        Task<Sale?> GetByNumberAsync(string number, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
