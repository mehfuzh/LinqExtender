using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Ast
{
    /// <summary>
    /// Abstract expression base that represents the LINQ query.
    /// </summary>
    public abstract class Expression 
    {
        /// <summary>
        /// Gets a value indicating the type of expression.
        /// </summary>
        public abstract CodeType CodeType { get; }
    }
}
