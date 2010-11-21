using System;
using LinqExtender.Attributes;

namespace LinqExtender.Tests
{
    [Name("ext_library")]
    public class Library
    {
        [Name("lb_Id")]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
