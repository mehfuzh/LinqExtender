using System.Collections.Generic;

namespace LinqExtender
{
    public interface IQueryContext<T>
    {
        /// <summary>
        /// Executes the current Linq query.
        /// </summary>
        /// <param name="exprssion"></param>
        /// <returns></returns>
        IEnumerable<T> Execute(Ast.Expression exprssion);
    }
}
