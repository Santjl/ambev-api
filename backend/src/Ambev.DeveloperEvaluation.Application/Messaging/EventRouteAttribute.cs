using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Messaging
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class EventRouteAttribute : Attribute
    {
        public string Name { get; }
        public EventRouteAttribute(string name) => Name = name;
    }

    public interface IEventPayload
    {
        object ToPayload();
    }
}
