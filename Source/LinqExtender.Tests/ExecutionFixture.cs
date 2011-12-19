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
        public void ShouldAssertExpectedResultFromGivenContext()
        {
            var query = from book in new FakeContext(GetBooks())
                        where book.Id == 2
                        select book;

            Assert.AreEqual(2, query.First().Id);
        }

        [Test]
        public void ShouldAssertProjectionSelect()
        {
            var query = new FakeContext(GetBooks())
                .Where(book => book.Id == 2)
                .Select(r => new { r.Id, r.Title });
       
            Assert.AreEqual(2, query.First().Id);
        }

        [Test]
        public void ShouldAssertTakeAndSkip()
        {
            var query = (from book in new FakeContext(GetBooks())
                        where book.Id == 2
                        select book).Take(1).Skip(0);

            Assert.AreEqual(2, query.First().Id);
        }

        [Test]
        public void ShouldNotCacheTakeOrSkipFromPreviousContext()
        {
            var expected = (from book in new FakeContext(GetBooks())select book).Skip(1).Take(1).ToArray();
            var actual = (from book in new FakeContext(GetBooks()) select book).ToArray();

            Assert.AreNotEqual(expected.Count(), actual.Count());
        }

        internal class FakeContext : ExpressionVisitor,  IQueryContext<Book>
        {
            internal FakeContext(IList<Book> source)
            {
                this.source = source;
                this.methodCalls = new List<Ast.MethodCallExpression>();
            }

            public IEnumerable<Book> Execute(Ast.Expression expression)
            {
                this.Visit(expression);

                var result = source.AsQueryable();

                if (this.expression != null)
                {
                    var lambda = Expression.Lambda(this.expression, new[] { parameter });
                    var func = (Func<Book, bool>)lambda.Compile();
                    result = source.Where(func).AsQueryable();
                }

                foreach (var methodCall in methodCalls)
                {
                    var parameters = new Expression[methodCall.Paramters.Length + 1];

                    parameters[0] = Expression.Constant(result);

                    for (int index = 0; index < methodCall.Paramters.Length; index++)
                    {
                        parameters[index + 1] = Expression.Constant(methodCall.Paramters[index].Value, methodCall.Paramters[index].Type);
                    }

                    var exp = Expression.Call(methodCall.Method, parameters);

                    result = (IQueryable<Book>)Expression.Lambda(exp).Compile().DynamicInvoke();
                }
                return result.AsEnumerable();
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
                this.expression = Expression.Constant(expression.Value, expression.Type.UnderlyingType);
                return expression;
            }

            public override Ast.Expression VisitMethodCallExpression(Ast.MethodCallExpression expression)
            {
                methodCalls.Add(expression);
                return expression;
            }

            private readonly IList<Book> source;
            private Expression expression;
            private IList<Ast.MethodCallExpression> methodCalls;
            private ParameterExpression parameter;
        }

        public IList<Book> GetBooks()
        {
            IList<Book> books = new List<Book>();

            books.Add(new Book { Id = 1, Author = "Scott" });
            books.Add(new Book { Id = 2, Author = "John" });

            return books;

        }
    }

}
