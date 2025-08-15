using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ambev.DeveloperEvaluation.Application.Common.Ports.DTos;

namespace Ambev.DeveloperEvaluation.Application.Common.Ports
{
    public interface IBranchGateway
    {
        Task<BranchDto?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
