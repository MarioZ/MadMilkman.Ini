using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a base generic class for INI content item collections, <see cref="IniSectionCollection"/> and <see cref="IniKeyCollection"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="IniItem"/> derived type.</typeparam>
    /// <seealso cref="IniItem"/>
    [DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(DebugCollectionViewer<>))]
    public abstract class IniItemCollection<T> : IItemNameVerifier, IList<T> where T : IniItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly bool caseSensitive;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniDuplication duplication;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IList<T> items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniFile parentFile;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniItem owner;

        /// <exclude/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IniFile ParentFile { get { return this.parentFile; } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IniItem Owner { get { return this.owner; } }

        /// 
        /// <summary>
        /// Gets the number of items in this collection.
        /// </summary>
        public int Count { get { return this.items.Count; } }

        internal IniItemCollection(IniFile parentFile, IniItem owner, IniDuplication duplication, bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
            this.duplication = duplication;
            this.parentFile = parentFile;
            this.owner = owner;
            this.items = new List<T>();
        }

        /// <summary>
        /// Adds an item to the end of this collection.
        /// </summary>
        /// <param name="item">Item to add to this collection.</param>
        /// <include file='IniInternal\SharedDocumentationComments.xml' path='Comments/Comment[@name="AddIgnored"]/*'/>
        public void Add(T item)
        {
            if (this.VerifyItem(item))
                this.items.Add(item);
        }

        /// <summary>
        /// Removes all items from this collection.
        /// </summary>
        public void Clear()
        {
            foreach (var item in this.items)
                item.ParentCollectionCore = null;
            this.items.Clear();
        }

        /// <summary>
        /// Determines whether an item is in this collection.
        /// </summary>
        /// <param name="item">Item to locate in this collection.</param>
        /// <returns><see langword="true"/> if the specified item is in the collection.</returns>
        public bool Contains(T item) { return this.items.Contains(item); }

        /// <summary>
        /// Determines whether an item is in this collection.
        /// </summary>
        /// <param name="name">Name of the item to locate in this collection.</param>
        /// <returns><see langword="true"/> if the item with specified name is in the collection.</returns>
        public bool Contains(string name) { return this.GetItemIndexByName(name) != -1; }
        
        /// <summary>
        /// Shallow copies the items of this collection to an array.
        /// </summary>
        /// <param name="array">One-dimensional array that is the destination of the items copied from this collection.</param>
        /// <param name="arrayIndex">Zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) { this.items.CopyTo(array, arrayIndex); }

        /// <summary>
        /// Searches for the specified item and returns the zero-based index of the first occurrence within this collection.
        /// </summary>
        /// <param name="item">Item to locate in this collection.</param>
        /// <returns>Index of the first occurrence of specified item in the collection.</returns>
        public int IndexOf(T item) { return this.items.IndexOf(item); }

        /// <summary>
        /// Searches for the specified item and returns the zero-based index of the first occurrence within this collection.
        /// </summary>
        /// <param name="name">Name of the item to locate in this collection.</param>
        /// <returns>Index of the first occurrence of the item with specified name in the collection.</returns>
        public int IndexOf(string name) { return this.GetItemIndexByName(name); }

        /// <summary>
        /// Inserts an item to this collection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted.</param>
        /// <param name="item">Item to insert to this collection.</param>
        /// <include file='IniInternal\SharedDocumentationComments.xml' path='Comments/Comment[@name="InsertIgnored"]/*'/>
        public void Insert(int index, T item)
        {
            if (this.VerifyItem(item))
                this.items.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of specific item from this collection.
        /// </summary>
        /// <param name="item">Item to remove from this collection.</param>
        /// <returns><see langword="true"/> if the specified item is removed from the collection.</returns>
        public bool Remove(T item)
        {
            if (!this.items.Remove(item))
                return false;

            item.ParentCollectionCore = null;
            return true;
        }

        /// <summary>
        /// Removes the first occurrence of specific item from this collection.
        /// </summary>
        /// <param name="name">Name of the item to remove from this collection.</param>
        /// <returns><see langword="true"/> if the item with specified name is removed from the collection.</returns>
        public bool Remove(string name)
        {
            int index = this.GetItemIndexByName(name);

            if (index == -1)
                return false;

            this.items.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes an item at the specified index from this collection.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted.</param>
        public void RemoveAt(int index)
        {
            this.items[index].ParentCollectionCore = null;
            this.items.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index of the item to get or set.</param>
        /// <remarks>
        /// If item duplicates are <see cref="IniDuplication.Ignored">ignored</see> and value is a duplicate item, has an existing name in this collection, then this value <b>is ignored</b>.
        /// </remarks>
        public T this[int index]
        {
            get { return this.items[index]; }
            set
            {
                if (this.VerifyItem(value))
                {
                    this.items[index].ParentCollectionCore = null;
                    this.items[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets the first item of the specified name.
        /// </summary>
        /// <param name="name">Name of the item to get.</param>
        /// <remarks>If item with the specified name doesn't exist a <see langword="null"/> value is returned.</remarks>
        public T this[string name]
        {
            get
            {
                int index = this.GetItemIndexByName(name);
                return (index != -1) ? this.items[index] : null;
            }
        }

        /// <summary>
        /// Gets the first items of the specified names.
        /// </summary>
        /// <param name="names">Names of the items to get.</param>
        /// <remarks>If item with any specified name doesn't exist a <see langword="null"/> value is returned in its place.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers",
         Justification = "I believe that this non-standard indexer can provide some useful data store access.")]
        public IEnumerable<T> this[params string[] names]
        {
            get
            {
                var returnedNames = new Dictionary<string, int>();
                int index;

                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];

                    if (returnedNames.TryGetValue(name, out index))
                    {
                        if (index != -1)
                        {
                            index = GetItemIndexByName(name, index + 1);
                            returnedNames[name] = index;
                        }
                    }
                    else
                    {
                        index = GetItemIndexByName(name);
                        returnedNames.Add(name, index);
                    }

                    yield return (index != -1) ? this.items[index] : null;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> object that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() { return this.items.GetEnumerator(); }

        private int GetItemIndexByName(string name, int startIndex = 0)
        {
            /* MZ(2016-08-25): Fixed issue with invalid cast exception. */
            for (int index = startIndex; index < this.items.Count; ++index)
                if (this.items[index].Name.Equals(name, (this.caseSensitive) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
                    return index;

            return -1;
        }

        private bool VerifyItem(IniItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.ParentFile != this.parentFile)
                throw new InvalidOperationException();

            if (item.ParentCollectionCore != null)
                throw new InvalidOperationException();

            if (!this.VerifyItemName(item.Name))
                return false;

            item.ParentCollectionCore = this;
            return true;
        }

        private bool VerifyItemName(string name) { return ((IItemNameVerifier)this).VerifyItemName(name); }

        bool IItemNameVerifier.VerifyItemName(string name)
        {
            if (this.duplication != IniDuplication.Allowed && this.Contains(name))
            {
                if (this.duplication == IniDuplication.Disallowed)
                    throw new InvalidOperationException();

                return false;
            }

            return true;
        }

        /// <exclude/>
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        /// <exclude/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool ICollection<T>.IsReadOnly { get { return false; } }
    }
}