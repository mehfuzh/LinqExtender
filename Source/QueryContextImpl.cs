using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqExtender.Abstraction;
using LinqExtender.Collection;

namespace LinqExtender
{
    internal class QueryContextImpl<T> : IQueryContextImpl<T>
    {
        public QueryContextImpl(IQueryContext<T> queryContext)
        {
            this.queryContext = queryContext;
            collection = new QueryCollection<T>();
        }

        public IModifiableCollection<T> Collection
        {
            get
            {
                return collection;
            }
        }

        IEnumerable<T> IQueryContext<T>.Execute(Ast.Expression exprssion)
        {
            return queryContext.Execute(exprssion);
        }

        private readonly IModifiableCollection<T> collection;
        private readonly IQueryContext<T> queryContext;
    }
}
