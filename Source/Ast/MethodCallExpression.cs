
namespace LinqExtender.Ast
{
    public class MethodCallExpression : Expression
    {
        internal MethodCallExpression(int skip, int? take)
        {
            Skip = skip;
            Take = take;
        }

        public int Skip { get; set; }
        public int? Take { get; set; }

        public override CodeType CodeType
        {
            get { return CodeType.MethodCallExpression; }
        }
    }
}
