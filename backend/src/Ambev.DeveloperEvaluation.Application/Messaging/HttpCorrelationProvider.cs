using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    public sealed class HttpCorrelationProvider : ICorrelationProvider
    {
        private Guid _corr = Guid.NewGuid();
        private Guid? _cause;
        public Guid CorrelationId => _corr;
        public Guid? CausationId => _cause;
        public void Set(Guid correlationId, Guid? causationId = null) { _corr = correlationId; _cause = causationId; }
    }
}
