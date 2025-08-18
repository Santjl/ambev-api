using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public static class EventsConsts
    {
        public const string SaleCreated = "sales.sale.created";
        public const string SaleModified = "sales.sale.modified";
        public const string SaleCancelled = "sales.sale.cancelled";
        public const string SaleItemCancelled = "sales.item.cancelled";
    }
}
