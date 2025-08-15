using Ambev.DeveloperEvaluation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        private readonly List<SaleItem> _items = new();

        public string Number { get; private set; } = default!;
        public DateTimeOffset Date { get; private set; } = DateTime.UtcNow;
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; } = default!;
        public Guid BranchId { get; private set; }
        public string BranchName { get; private set; } = default!;
        public decimal Total { get; private set; } = 0;
        public bool IsCancelled { get; private set; }
        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
        public Sale()
        {
        }

        public static Sale Create(
        string number,
        DateTimeOffset date,
        Guid customerId, string customerName,
        Guid branchId, string branchName)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new DomainException("Sale number is required.");
            return new Sale
            {
                Number = number.Trim(),
                Date = date,
                CustomerId = customerId,
                CustomerName = customerName.Trim(),
                BranchId = branchId,
                BranchName = branchName.Trim(),
                Total = 0m,
                IsCancelled = false
            };
        }

        public void AddItem(Guid productId, string productName, int qty, decimal unitPrice)
        {
            EnsureNotCancelled();
            var item = SaleItem.Create(productId, productName, qty, unitPrice);
            _items.Add(item);
            RecalculateTotal();
        }

        public void UpdateItem(Guid itemId, int qty, decimal unitPrice)
        {
            EnsureNotCancelled();
            var item = _items.SingleOrDefault(i => i.Id == itemId) ?? throw new DomainException("Item not found.");
            item.Update(qty, unitPrice);
            RecalculateTotal();
        }

        public void CancelItem(Guid itemId)
        {
            EnsureNotCancelled();
            var item = _items.SingleOrDefault(i => i.Id == itemId) ?? throw new DomainException("Item not found.");
            item.Cancel();
            RecalculateTotal();
        }

        public void RemoveItem(Guid itemId)
        {
            EnsureNotCancelled();
            _items.RemoveAll(i => i.Id == itemId);
            RecalculateTotal();
        }

        public void Cancel()
        {
            if (IsCancelled) return;
            IsCancelled = true;

            foreach (var item in _items.Where(i => !i.IsCancelled))
                item.Cancel();
        }

        private void RecalculateTotal()
        {
            Total = _items.Where(i => !i.IsCancelled).Sum(i => i.Total);
            Total = Math.Round(Total, 2, MidpointRounding.ToEven);
        }

        private void EnsureNotCancelled()
        {
            if (IsCancelled) throw new DomainException("Sale is cancelled.");
        }
    }
}
