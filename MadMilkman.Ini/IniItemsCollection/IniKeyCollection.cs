using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a collection of <see cref="IniKey"/> items.
    /// </summary>
    /// <seealso cref="IniKey"/>
    public sealed class IniKeyCollection : IniItemCollection<IniKey>
    {
        internal IniKeyCollection(IniFile parentFile, IniDuplication duplication, bool caseSensitive)
            : base(parentFile, duplication, caseSensitive) { }

        /// <summary>
        /// Adds an item to the end of this collection.
        /// </summary>
        /// <param name="name">Name of the <see cref="IniKey"/> to add to this collection.</param>
        /// <returns><see cref="IniKey"/> that was added to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="AddIgnored"]/*'/>
        public IniKey Add(string name) { return this.Add(name, null); }

        /// <summary>
        /// Adds an item to the end of this collection.
        /// </summary>
        /// <param name="nameValuePair">The key's data, pair of key's name and key's value, to add to this collection.</param>
        /// <returns><see cref="IniKey"/> that was added to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="AddIgnored"]/*'/>
        public IniKey Add(KeyValuePair<string, string> nameValuePair) { return this.Add(nameValuePair.Key, nameValuePair.Value); }

        /// <summary>
        /// Adds an item to the end of this collection.
        /// </summary>
        /// <param name="name">Name of the <see cref="IniKey"/> to add to this collection.</param>
        /// <param name="value">Value of the <see cref="IniKey"/> to add to this collection.</param>
        /// <returns><see cref="IniKey"/> that was added to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="AddIgnored"]/*'/>
        public IniKey Add(string name, string value)
        {
            var key = new IniKey(this.ParentFile, name, value);
            this.Add(key);
            return key;
        }

        /// <summary>
        /// Inserts an item to this collection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted.</param>
        /// <param name="name">Name of the <see cref="IniKey"/> to insert to this collection.</param>
        /// <returns><see cref="IniKey"/> that was inserted to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="InsertIgnored"]/*'/>
        public IniKey Insert(int index, string name) { return this.Insert(index, name, null); }

        /// <summary>
        /// Inserts an item to this collection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted.</param>
        /// <param name="nameValuePair">The key's data, pair of key's name and key's value, to insert to this collection.</param>
        /// <returns><see cref="IniKey"/> that was inserted to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="InsertIgnored"]/*'/>
        public IniKey Insert(int index, KeyValuePair<string, string> nameValuePair){ return this.Insert(index, nameValuePair.Key, nameValuePair.Value); }

        /// <summary>
        /// Inserts an item to this collection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted.</param>
        /// <param name="name">Name of the <see cref="IniKey"/> to insert to this collection.</param>
        /// <param name="value">Value of the <see cref="IniKey"/> to insert to this collection.</param>
        /// <returns><see cref="IniKey"/> that was inserted to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="InsertIgnored"]/*'/>
        public IniKey Insert(int index, string name, string value)
        {
            var key = new IniKey(this.ParentFile, name, value);
            this.Insert(index, key);
            return key;
        }
    }
}