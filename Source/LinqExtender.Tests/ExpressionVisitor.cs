using System;

namespace LinqExtender.Tests
{
    public class ExpressionVisitor
    {
        internal Ast.Expression Visit(Ast.Expression expression)
        {
            switch (expression.CodeType)
            {
                case CodeType.BlockExpression:
                    return VisitBlockExpression((Ast.BlockExpression)expression);
                case CodeType.TypeExpression:
                    return VisitTypeExpression((Ast.TypeExpression)expression);
                case CodeType.LambdaExpresion:
                    return VisitLambdaExpression((Ast.LambdaExpression)expression);
                case CodeType.LogicalExpression:
                    return VisitLogicalExpression((Ast.LogicalExpression)expression);
                case CodeType.BinaryExpression:
                    return VisitBinaryExpression((Ast.BinaryExpression)expression);
                case CodeType.LiteralExpression:
                    return VisitLiteralExpression((Ast.LiteralExpression)expression);
                case CodeType.MemberExpression:
                    return VisitMemberExpression((Ast.MemberExpression)expression);
                case CodeType.OrderbyExpression:
                    return VisitOrderbyExpression((Ast.OrderbyExpression)expression);
                case CodeType.MethodCallExpression:
                    return VisitMethodCallExpression((Ast.MethodCallExpression)expression);
            }

            throw new ArgumentException("Expression type is not supported");
        }

        public virtual Ast.Expression VisitTypeExpression(Ast.TypeExpression typeExpression)
        {
            return typeExpression;
        }

        public virtual Ast.Expression VisitBlockExpression(Ast.BlockExpression blockExpression)
        {
            foreach (var expression in blockExpression.Expressions)
                this.Visit(expression);

            return blockExpression;
        }

        public virtual Ast.Expression VisitMethodCallExpression(Ast.MethodCallExpression methodCallExpression)
        {
            return methodCallExpression;
        }

        public virtual Ast.Expression VisitLogicalExpression(Ast.LogicalExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);
            return expression;
        }

        public virtual Ast.Expression VisitLambdaExpression(Ast.LambdaExpression expression)
        {
            if (expression.Body != null)
                return this.Visit(expression.Body);
            return expression;
        }

        public virtual Ast.Expression VisitBinaryExpression(Ast.BinaryExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);

            return expression;
        }

        public virtual Ast.Expression VisitMemberExpression(Ast.MemberExpression expression)
        {
            return expression;
        }

        public virtual Ast.Expression VisitLiteralExpression(Ast.LiteralExpression expression)
        {
            return expression;
        }

        public virtual Ast.Expression VisitOrderbyExpression(Ast.OrderbyExpression expression)
        {
            return expression;
        }

    }
}
