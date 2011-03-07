using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace LinqExtender.Tests
{
    public class BaseFixture
    {
        protected string Expected()
        {
            var frame = new StackTrace(1).GetFrame(0);

            // get the target test method.
            var targetMethod = frame.GetMethod();

            var expected = ReadResult(targetMethod.Name);
            return expected;
        }

        protected string Source(StringBuilder builder)
        {
            string content = builder.ToString();
            return Regex.Replace(content, "[\r\n\t]", string.Empty);
        }

        private string ReadResult(string testname)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(String.Format("LinqExtender.Tests.Cases.{0}.txt", testname)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    return Regex.Replace(content, "[\r\n\t]", string.Empty);
                }
            }
        }
    }
}
