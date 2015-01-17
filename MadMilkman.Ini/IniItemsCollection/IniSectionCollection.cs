using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a collection of <see cref="IniSection"/> items.
    /// </summary>
    public sealed class IniSectionCollection : IniItemCollection<IniSection>
    {
        internal IniSectionCollection(IniFile parentFile, IniDuplication duplication, bool caseSensitive)
            : base(parentFile, duplication, caseSensitive) { }

        /// <summary>
        /// Adds an item to the end of this collection.
        /// </summary>
        /// <param name="name">Name of the <see cref="IniSection"/> to add to this collection.</param>
        /// <returns><see cref="IniSection"/> that was added to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="AddIgnored"]/*'/>
        public IniSection Add(string name)
        {
            var section = new IniSection(this.parentFile, name);
            this.Add(section);
            return section;
        }

        /// <summary>
        /// Inserts an item to this collection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which item should be inserted.</param>
        /// <param name="name">Name of the <see cref="IniSection"/> to insert to this collection.</param>
        /// <returns><see cref="IniSection"/> that was inserted to this collection.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="InsertIgnored"]/*'/>
        public IniSection Insert(int index, string name)
        {
            var section = new IniSection(this.parentFile, name);
            this.Insert(index, section);
            return section;
        }
    }
}
