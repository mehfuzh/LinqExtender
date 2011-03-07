using System.Linq;
using System.Text;
using NUnit.Framework;
using System;

namespace LinqExtender.Tests
{
    [TestFixture]
    public class LinqQueryTest : BaseFixture
    {
        [Test]
        public void ShouldAssertSimpleSelect()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        select book;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertSimpleWhereClause()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where book.Id == 1
                        select book;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertSimpleWhereWithLogicalExpression()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where book.Id == 1 && book.Author == "Charlie"
                        select book;


            query.Count();

            Assert.AreEqual(Expected(), Source(builder));

        }

        [Test]
        public void ShouldAssertWhereWithNestedLeftLogicalExpression()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where (book.Id == 1 && book.Author == "Charlie")
                        || book.Id == 10
                        select book;


            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertWhereWithtNestedRightLogicalExpression()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where book.Id == 10 || (book.Id == 1 && book.Author == "Charlie")
                        select book;


            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertWhereWithNestedLeftAndRightLogicaExpression()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where (book.Id == 10 && book.Author == "Plarosi") || (book.Id == 1 && book.Author == "Charlie")
                        select book;


            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertSimpleOrderBySelect()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        orderby book.Author
                        select book;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertOrderByDescending()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        orderby book.Author descending
                        select book;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertSimpleWhereThatHasMethodCall()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where book.Author == GetAuthor(1)
                        select book;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldConcatMultipleWhereCallsWithLogicalAnd()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = context.Where(x => x.Id == 1).Where(x => x.Author == "Scott");

            query.FirstOrDefault();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldJoinWhereUsingAndWhenNextCallHavingLogicalExpr()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = context
                .Where(x => x.ISBN == "111")
                .Where(x => x.Id == 1 || x.Author == "Scott" || x.IsAvailable);


            query.FirstOrDefault();

            Assert.AreEqual(Expected(), Source(builder));
        }

        // Fake method to test the capability of using
        // method call in the query.
        private string GetAuthor(int bookId)
        {
            if (bookId > 0)
                return "Tom";

            throw new ArgumentException("Not a valid book id");
        }
    }
}
