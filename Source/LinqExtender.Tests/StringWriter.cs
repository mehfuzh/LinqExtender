using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Tests
{
    public class StringWriter : ITextWriter
    {
        public StringWriter(StringBuilder builder)
        {
            this.builder = builder;
        }

        public void Write(object value)
        {
            builder.Append(value);
        }

        private StringBuilder builder;
    }
}
