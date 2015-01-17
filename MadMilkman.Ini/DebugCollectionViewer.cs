using System.Diagnostics;
using System.Collections.Generic;

namespace MadMilkman.Ini
{
    internal sealed class DebugCollectionViewer<T>
    {
        private readonly IEnumerable<T> sequence;

        public DebugCollectionViewer(IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new System.ArgumentNullException("sequence");
            this.sequence = sequence;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                var collection = this.sequence as ICollection<T> ?? new List<T>(this.sequence);
                var array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}
