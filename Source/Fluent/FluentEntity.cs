using System;
using LinqExtender.Abstraction;

namespace LinqExtender.Fluent
{
    /// <summary>
    /// Contains Entity Info.
    /// </summary>
    public class FluentEntity
    {
        /// <summary>
        /// Creates a new instance of <see cref="FluentEntity"/>
        /// </summary>
        /// <param name="bucket"></param>
        internal FluentEntity(IBucket bucket)
        {
            this.bucket = bucket;   
        }
        /// <summary>
        ///  Name of the entity, can be overriden by <c>OriginalEntityNameAttribute</c>
        /// </summary>
        public string Name
        {
            get
            {
                return bucket.Name;
            }
        }
        /// <summary>
        /// Gets items to fetch from source.
        /// </summary>
        public int? ItemsToFetch
        {
            get
            {
                return bucket.ItemsToTake;
            }
        }
        /// <summary>
        /// Default  0, number of items to skip from start.
        /// </summary>
        public int ItemsToSkipFromStart
        {
            get
            {
                return bucket.ItemsToSkip;
            }
        }
        /// <summary>
        /// List of unique column name.
        /// </summary>
        public string UniqueAttribte
        {
            get
            {
                return bucket.UniqueItems[0];
            }
        }
        /// <summary>
        /// Defines a fluent implentation for order by query.
        /// </summary>
        public class FluentOrderBy
        {
            /// <summary>
            /// Creates a new instance of <see cref="FluentOrderBy"/>
            /// </summary>
            /// <param name="bucket"></param>
            internal FluentOrderBy(IBucket bucket)
            {
                this.bucket = bucket;
            }
            
            /// <summary>
            /// Checks if orderby is used in query and calls action delegate to 
            /// execute user's code and internally marks <value>true</value> for ifUsed field
            /// to be used by <see cref="FluentOrderByItem"/> iterator.
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public FluentOrderBy IfUsed(Action action)
            {
                ifUsed = bucket.OrderByItems.Count > 0;

                if (ifUsed && action != null)
                    action.DynamicInvoke();
                return this;
            }
            /// <summary>
            /// Iterator for order by items.
            /// </summary>
            public FluentOrderByItem ForEach
            {
                get
                {
                    return new FluentOrderByItem(bucket);
                }
            }
         
            private bool ifUsed = false;
            /// <summary>
            /// Callback handler for <see cref="FluentOrderBy"/>
            /// </summary>
            /// <param name="member">Target member</param>
            /// <param name="ascending">bool for sort order</param>
            public delegate void Callback(MemberReference member, bool ascending);
            private readonly IBucket bucket;

            /// <summary>
            /// Order by iterator.
            /// </summary>
            public class FluentOrderByItem
            {
                /// <summary>
                /// Creates a new instance of <see cref="FluentOrderBy"/>
                /// </summary>
                /// <param name="bucket"></param>
                internal FluentOrderByItem(IBucket bucket)
                {
                    this.bucket = bucket;
                }

                /// <summary>
                /// Does a callback to process the order by used in where clause.
                /// </summary>
                /// <param name="callback"></param>
                public void Process(Callback callback)
                {
                    foreach (Bucket.OrderByInfo info in bucket.OrderByItems)
                    {
                        callback.Invoke(info.Member, info.IsAscending);
                    }
                }

                private readonly IBucket bucket;
            }
        }

        /// <summary>
        /// Gets an intance for the <see cref="FluentOrderBy"/>
        /// </summary>
        public FluentOrderBy OrderBy
        {
            get
            {
                return new FluentOrderBy(bucket);
            }
        }

        private readonly IBucket bucket;
    }
}