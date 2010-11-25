using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Ast
{
    public class LiteralExpression : Expression
    {
        private object value;
        private TypeReference type;

        public LiteralExpression(Type type, object value)
        {
            this.type = new TypeReference(type);
            this.value = value;
        }

        /// <summary>
        /// Gets type of the expression.
        /// </summary>
        public TypeReference Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Gets the value that is evaulated from linq query.
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }
        }

        public override CodeType CodeType
        {
            get { return CodeType.LiteralExpression; }
        }
    }
}
