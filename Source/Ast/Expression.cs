using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Ast
{
    public abstract class Expression 
    {
        /// <summary>
        /// Gets the type for the expression.
        /// </summary>
        public abstract CodeType CodeType { get; }
    }
}
