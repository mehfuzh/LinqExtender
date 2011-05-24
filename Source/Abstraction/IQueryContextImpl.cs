using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Abstraction
{
    internal interface IQueryContextImpl<T> : IQueryContext<T>
    {
        IModifiableCollection<T> Collection { get; }
    }
}
