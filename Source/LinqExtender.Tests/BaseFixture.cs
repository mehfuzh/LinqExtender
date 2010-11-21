using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using NUnit.Framework;

namespace LinqExtender.Tests
{
    public class BaseFixture
    {
        protected string ReadFromFile()
        {
            var frame = new StackTrace(1).GetFrame(0);

            // get the target test method.
            var targetMethod = frame.GetMethod();

            var expected = ReadResult(targetMethod.Name);
            return expected;
        }

        private string ReadResult(string testname)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream("LinqExtender.Tests.Scripts." + testname + ".sql"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
