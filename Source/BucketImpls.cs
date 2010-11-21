using System.Collections.Generic;

namespace LinqExtender
{
    internal class BucketImpls<T> : Dictionary<string, BucketImpl>
    {
        public BucketImpl Current
        {
            get
            {
                EnsureWorkingItem(typeof(T).FullName);
                return current;
            }
            set
            {
                this[typeof (T).FullName] = value; 
            }
        }

        private void EnsureWorkingItem(string currentKey)
        {
            if (this.ContainsKey(currentKey))
            {
                // set the current object.
                current = this[currentKey].InstanceImpl;
            }
        }

        private BucketImpl current = null;
    
    }
}