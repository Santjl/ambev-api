using Ambev.DeveloperEvaluation.Application.Common.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ambev.DeveloperEvaluation.Application.Common.Ports.DTos;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class InMemoryExternalCatalogGateway :
    IProductGateway, IBranchGateway, ICustomerGateway
    {
        private readonly Dictionary<Guid, ProductDto> _products = new()
        {
            [Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")] = new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Mouse", 50m),
            [Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")] = new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "USB Cable", 10m),
            [Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")] = new(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Keyboard", 120m),
        };

        private readonly Dictionary<Guid, BranchDto> _branches = new()
        {
            [Guid.Parse("22222222-2222-2222-2222-222222222222")] = new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Downtown"),
            [Guid.Parse("33333333-3333-3333-3333-333333333333")] = new(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Airport"),
        };

        private readonly Dictionary<Guid, CustomerDto> _customers = new()
        {
            [Guid.Parse("11111111-1111-1111-1111-111111111111")] = new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Alice Smith"),
            [Guid.Parse("44444444-4444-4444-4444-444444444444")] = new(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Bob Johnson"),
        };

        Task<ProductDto?> IProductGateway.GetByIdAsync(Guid id, CancellationToken ct) =>
            Task.FromResult(_products.TryGetValue(id, out var p) ? p : null);

        Task<BranchDto?> IBranchGateway.GetByIdAsync(Guid id, CancellationToken ct) =>
            Task.FromResult(_branches.TryGetValue(id, out var b) ? b : null);

        Task<CustomerDto?> ICustomerGateway.GetByIdAsync(Guid id, CancellationToken ct) =>
            Task.FromResult(_customers.TryGetValue(id, out var c) ? c : null);

        public IEnumerable<Guid> Products => _products.Keys;
        public IEnumerable<Guid> Branches => _branches.Keys;
        public IEnumerable<Guid> Customers => _customers.Keys;
    }
}
