using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace LinqExtender.Tests
{
    [TestFixture]
    public class CustomAttributeTest : BaseFixture 
    {
        [Test]
        public void ShouldAssertObjectNameAsSpecifiedInNameAttribute()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Library>(new StringWriter(builder));

            var query = from libary in context
                        select libary;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }

        [Test]
        public void ShouldAssertPropertyNameAsSpeficiedInNameAttribute()
        {
            var builder = new StringBuilder();
            var context = new TextContext<Library>(new StringWriter(builder));

            var query = from libary in context
                        where libary.Id == 2
                        select libary;

            query.Count();

            Assert.AreEqual(Expected(), Source(builder));
        }
    }
}
