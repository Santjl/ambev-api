using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Common
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class EventNameAttribute : Attribute
    {
        public string Name { get; }
        public EventNameAttribute(string name) => Name = name;
    }
}
