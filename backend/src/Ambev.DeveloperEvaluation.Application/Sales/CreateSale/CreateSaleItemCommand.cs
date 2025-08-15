using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleItemCommand
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
