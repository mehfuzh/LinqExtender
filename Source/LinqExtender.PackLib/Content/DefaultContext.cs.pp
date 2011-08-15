using System;
using System.Collections.Generic;
using LinqExtender;
using Ast = LinqExtender.Ast;

namespace $rootnamespace$
{
    /// <summary>
    /// Default context to be queried.
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    public class DefaultContext<T> : ExpressionVisitor, IQueryContext<T>
    {
        /// <summary>
        /// Invoked during execution of the query , with the
        /// pre populated expression tree.
        /// </summary>
        /// <param name="expression">Target expression block</param>
        /// <returns>Expected result</returns>
        public IEnumerable<T> Execute(Ast.Expression expression)
        {
            //TODO: Visit the extender expression to build your meta 
            
            this.Visit(expression);

            ///TOOD: return your result.
            return null;
        }
    }
}
