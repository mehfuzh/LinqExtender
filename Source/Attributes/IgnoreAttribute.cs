using System;

namespace LinqExtender.Attributes
{
    /// <summary>
    /// Under this attribute present, property will be ignored by extender.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : System.Attribute
    {
  
    }
}