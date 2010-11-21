using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqExtender.Attributes
{
    /// <summary>
    ///  Defines a property to be unique.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UniqueIdentifierAttribute : System.Attribute
    {

    }
}