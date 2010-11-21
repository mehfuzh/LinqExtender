using System;

namespace LinqExtender.Tests
{
    public class Book
    {
        public string Author { get; set; }

        public string Title { get; set; }

        public string ISBN { get; set; }

        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Identity inherits Unique Attribute, these will be useful for update query.
        /// </summary>
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
    }
}
