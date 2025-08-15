using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IEnumerable<object> domainEvents, CancellationToken ct);
    }
}
