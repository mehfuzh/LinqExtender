using System;
using System.Collections.Generic;

namespace LinqExtender.Ast
{
    /// <summary>
    /// Combines a number expressions sequentially
    /// </summary>
    public class BlockExpression : Ast.Expression
    {
        public BlockExpression()
        {
            expressions = new List<Expression>();
        }

        public IList<Expression> Expressions
        {
            get
            {
                return expressions;
            }
        }

        public override CodeType CodeType
        {
            get { return CodeType.BlockExpression; }
        }

        private IList<Expression> expressions;
    }
}
