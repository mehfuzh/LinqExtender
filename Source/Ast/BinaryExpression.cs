namespace LinqExtender.Ast
{
    public class BinaryExpression : Expression
    {
        public BinaryOperator @operator;
        private Expression left;
        private Expression right;

        public BinaryExpression(BinaryOperator @operator)
        {
            this.@operator = @operator;
        }

        public Expression Left
        {
            get
            {
                return left;
            }
            internal set
            {
                left = value;
            }
        }

        public Expression Right
        {
            get
            {
                return right;
            }
            internal set
            {
                right = value;
            }
        }

        public override CodeType CodeType
        {
            get { return CodeType.BinaryExpression; }
        }
    }
}
