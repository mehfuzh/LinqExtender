using System.Collections.Generic;

namespace LinqExtender
{
    /// <summary>
    /// Entry point interface for defining a custom provider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
