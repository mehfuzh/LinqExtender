using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Ast
{
    public class LambdaExpression : Expression 
    {
        public LambdaExpression(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// Gets the underlying type of the expression.
        /// </summary>
        public Type Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Gets the body of the expression.
        /// </summary>
        public Expression Body
        {
            get
            {
                return body;
            }
            internal set
            {
                body = value;
            }
        }

        public override CodeType CodeType
        {
            get { return CodeType.LambdaExpresion; }
        }

        Type type;
        Expression body;
     
    }
}
