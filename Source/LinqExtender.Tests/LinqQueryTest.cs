using System.Linq;
using System.Text;
using NUnit.Framework;

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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
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

            Assert.AreEqual(ReadFromFile(), builder.ToString());
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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
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

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
        }

        [Test]
        public void ShouldAssertSimpleWhereThatHasMethodCall()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Book>(new StringWriter(builder));

            var query = from book in context
                        where book.Author == GetAuthorName()
                        select book;

            query.Count();

            string result = builder.ToString();

            Assert.AreEqual(ReadFromFile(), result);
        }

        private string GetAuthorName()
        {
            return "Tom";
        }
    }
}
