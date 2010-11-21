using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace LinqExtender.Tests
{
    [TestFixture]
    public class ExecutionFixture
    {
        [Test]
        public void ShouldAssertResultAgainstContextWithPreDefinedStore()
        {
            var query = from book in new FakeContext(GetBooks())
                        where book.Id == 2
                        select book;

            Assert.AreEqual(2, query.First().Id);
        }

        internal class FakeContext : ExpressionVisitor,  IQueryContext<Book>
        {
            internal FakeContext(IList<Book> source)
            {
                this.source = source;
            }

            public IEnumerable<Book> Execute(Ast.Expression expression)
            {
                this.Visit(expression);

                var lambda = Expression.Lambda(this.expression, new[] { parameter });
                var func = (Func<Book, bool>) lambda.Compile();

                return source.Where(func).AsEnumerable();
            }

            public override Ast.Expression VisitTypeExpression(Ast.TypeExpression expression)
            {
                parameter = Expression.Parameter(expression.Type.UnderlyingType, "x");
                return expression;
            }

            public override Ast.Expression VisitBinaryExpression(Ast.BinaryExpression expression)
            {
                this.Visit(expression.Left);

                var left = this.expression;
              
                this.Visit(expression.Right);

                var right = this.expression;

                this.expression = Expression.MakeBinary(ExpressionType.Equal, left, right);

                return expression;
            }

            public override Ast.Expression VisitMemberExpression(Ast.MemberExpression expression)
            {
                this.expression = Expression.MakeMemberAccess(parameter, expression.Member.MemberInfo);
                return expression;
            }


            public override Ast.Expression VisitLiteralExpression(Ast.LiteralExpression expression)
            {
                this.expression = Expression.Constant(expression.Value, expression.Type);
                return expression;
            }

            private IList<Book> source;
            private Expression expression;
            private ParameterExpression parameter;
        }

        public IList<Book> GetBooks()
        {
            IList<Book> books = new List<Book>();

            books.Add(new Book { Id = 1, Author = "Steve" });
            books.Add(new Book { Id = 2, Author = "John" });

            return books;

        }
    }

}
