using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LinqExtender.Tests
{
    public class TextContext<T> : ExpressionVisitor, IQueryContext<T>
    {
        public TextContext(ITextWriter writer)
        {
            this.writer = writer;
        }

        public IEnumerable<T> Execute(Ast.Expression expression)
        {
            this.Visit(expression);
            return new List<T>().AsEnumerable();
        }

        public override Ast.Expression VisitTypeExpression(Ast.TypeExpression expression)
        {
            writer.Write(string.Format("select * from {0}", expression.Type.Name));
            return expression;
        }

        public override Ast.Expression VisitLambdaExpression(Ast.LambdaExpression expression)
        {
            WriteNewLine();
            writer.Write("where");
            WriteNewLine();
            
            this.Visit(expression.Body);

            return expression;
        }

        public override Ast.Expression VisitBinaryExpression(Ast.BinaryExpression expression)
        {
            this.Visit(expression.Left);
            writer.Write(GetBinaryOperator(expression.Operator));
            this.Visit(expression.Right);

            return expression;
        }

        public override Ast.Expression VisitLogicalExpression(Ast.LogicalExpression expression)
        {
            WriteTokenIfReq(expression, Token.LeftParenthesis);
            
            this.Visit(expression.Left);

            WriteLogicalOperator(expression.Operator);

            this.Visit(expression.Right);

            WriteTokenIfReq(expression, Token.RightParentThesis);
       
            return expression;
        }

        public override Ast.Expression VisitMemberExpression(Ast.MemberExpression expression)
        {
            writer.Write(expression.FullName);
            return expression;
        }

        public override Ast.Expression VisitLiteralExpression(Ast.LiteralExpression expression)
        {
            WriteValue(expression.Type, expression.Value);
            return expression;
        }

        public override Ast.Expression VisitOrderbyExpression(Ast.OrderbyExpression expression)
        {
            WriteNewLine();
            Write(string.Format("order by {0}.{1} {2}", 
                expression.Member.DeclaringType.Name,
                expression.Member.Name, 
                expression.Ascending ? "asc" : "desc"));
            WriteNewLine();

            return expression;
        }

        private static string GetBinaryOperator(BinaryOperator @operator)
        {
            switch (@operator)
            {
                case BinaryOperator.Equal:
                    return " = ";
            }
            throw new ArgumentException("Invalid binary operator");
        }

        private void WriteLogicalOperator(LogicalOperator logicalOperator)
        {
            WriteSpace();

            writer.Write(logicalOperator.ToString().ToUpper());
      
            WriteSpace();
        }

        private void WriteSpace()
        {
            writer.Write(" ");
        }

        private void WriteNewLine()
        {
            writer.Write(Environment.NewLine);
        }

        private void WriteTokenIfReq(Ast.LogicalExpression expression, Token token)
        {
            if (expression.IsChild)
            {
                WriteToken(token);
            }
        }

        private void WriteToken(Token token)
        {
            switch (token)
            {
                case Token.LeftParenthesis:
                    writer.Write("(");
                    break;
                case Token.RightParentThesis:
                    writer.Write(")");
                    break;
            }
        }

        public enum Token
        {
            LeftParenthesis,
            RightParentThesis
        }

        private void WriteValue(TypeReference type, object value)
        {
           if (type.UnderlyingType == typeof(string))
               writer.Write(String.Format("\"{0}\"", value));
           else
               writer.Write(value);
        }

        private void Write(string value)
        {
            writer.Write(value);
        }

        public ITextWriter writer;
        public bool parameter;
    }
}
