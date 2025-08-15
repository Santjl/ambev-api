using Ambev.DeveloperEvaluation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = default!;

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal DiscountPercent { get; private set; }
        public decimal Total { get; private set; }
        public bool IsCancelled { get; private set; }

        private SaleItem() { }

        public static SaleItem Create(Guid productId, string productName, int qty, decimal unitPrice)
        {
            var si = new SaleItem
            {
                ProductId = productId,
                ProductName = productName.Trim()
            };
            si.Apply(qty, unitPrice);
            AddDomainEvent();
            return si;
        }

        public void Update(int qty, decimal unitPrice) => Apply(qty, unitPrice);

        public void Cancel() => IsCancelled = true;

        private void Apply(int qty, decimal unitPrice)
        {
            if (qty < 1) throw new DomainException("Quantity must be at least 1.");
            if (qty > 20) throw new DomainException("Cannot sell more than 20 identical items.");
            if (unitPrice < 0) throw new DomainException("Unit price cannot be negative.");

            Quantity = qty;
            UnitPrice = Math.Round(unitPrice, 2, MidpointRounding.ToEven);
            DiscountPercent = DiscountRulesForQuantity(qty);
            var gross = Quantity * UnitPrice;
            var net = gross * (1 - DiscountPercent);
            Total = Math.Round(net, 2, MidpointRounding.ToEven);
        }

        private static decimal DiscountRulesForQuantity(int qty)
        {
            if (qty < 4) return 0.00m;
            if (qty < 10) return 0.10m;
            if (qty <= 20) return 0.20m;
            throw new DomainException("Qty above 20 is not allowed.");
        }
    }
}
