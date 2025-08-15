using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Common.Ports
{
    public class DTos
    {
        public sealed record ProductDto(Guid Id, string Name, decimal Price);
        public sealed record BranchDto(Guid Id, string Name);
        public sealed record CustomerDto(Guid Id, string Name);
    }
}
