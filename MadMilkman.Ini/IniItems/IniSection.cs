using System.Diagnostics;
using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a section item of the INI file with name and keys content.
    /// </summary>
    public sealed class IniSection : IniItem
    {
        /// <summary>
        /// Represents a section name which is used to define a global section, used for storing first keys series that don't belong to any section.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a section with this name is located as a first file's section then its name and comments are ignored.
        /// If a section with this name isn't located as first file's section then it will be written with <c>MADMILKMAN_INI_FILE_GLOBAL_SECTION</c> name.
        /// </para>
        /// </remarks>
        public const string GlobalSectionName = "MADMILKMAN_INI_FILE_GLOBAL_SECTION";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniKeyCollection keys;

        /// <summary>
        /// Gets the <see cref="IniSection">section's</see> key collection.
        /// </summary>
        public IniKeyCollection Keys { get { return this.keys; } }

        /// <summary>
        /// Gets the <see cref="IniSectionCollection"/> to which this <see cref="IniSection"/> belongs to.
        /// </summary>
        public IniSectionCollection ParentCollection { get { return (IniSectionCollection)this.ParentCollectionCore; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="name">The section's name.</param>
        public IniSection(IniFile parentFile, string name) : this(parentFile, name, (IEnumerable<IniKey>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="name">The section's name.</param>
        /// <param name="keys">The section's keys.</param>
        public IniSection(IniFile parentFile, string name, params IniKey[] keys) : this(parentFile, name, (IEnumerable<IniKey>)keys) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="name">The section's name.</param>
        /// <param name="nameValuePairs">The section's keys data, pairs of key's name and key's value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
         Justification = "I don't want to use IDictionary<string, string>, there is no need for such a contract because IEnumerable<KeyValuePair<string, string>> is enough.")]
        public IniSection(IniFile parentFile, string name, IEnumerable<KeyValuePair<string, string>> nameValuePairs)
            : this(parentFile, name, GetIniKeysFromKeyValuePairs(parentFile, nameValuePairs)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniSection"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="name">The section's name.</param>
        /// <param name="keys">The section's keys.</param>
        public IniSection(IniFile parentFile, string name, IEnumerable<IniKey> keys)
            : base(parentFile, name)
        {
            this.keys = new IniKeyCollection(parentFile, this, parentFile.options.KeyDuplicate, parentFile.options.KeyNameCaseSensitive);

            if (keys != null)
                foreach (IniKey key in keys)
                    this.keys.Add(key);
        }

        // Constructor used by IniReader.
        internal IniSection(IniFile parentFile, string name, IniComment trailingComment)
            : base(parentFile, name, trailingComment)
        {
            this.keys = new IniKeyCollection(parentFile, this, parentFile.options.KeyDuplicate, parentFile.options.KeyNameCaseSensitive);
        }

        // Deep copy constructor.
        internal IniSection(IniFile destinationFile, IniSection sourceSection)
            : base(destinationFile, sourceSection)
        {
            this.keys = new IniKeyCollection(destinationFile, this, destinationFile.options.KeyDuplicate, destinationFile.options.KeyNameCaseSensitive);

            foreach (var key in sourceSection.keys)
                this.keys.Add(key.Copy(destinationFile));
        }

        /// <summary>
        /// Copies this <see cref="IniSection"/> instance.
        /// </summary>
        /// <returns>Copied <see cref="IniSection"/>.</returns>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#copying" target="_self">IniItem's Copying</seealso>
        public IniSection Copy() { return this.Copy(this.ParentFile); }

        /// <summary>
        /// Copies this <see cref="IniSection"/> instance and sets copied instance's <see cref="IniItem.ParentFile">ParentFile</see>.
        /// </summary>
        /// <param name="destinationFile">Copied section's parent file.</param>
        /// <returns>Copied <see cref="IniSection"/> that belongs to a specified <see cref="IniFile"/>.</returns>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#copying" target="_self">IniItem's Copying</seealso>
        public IniSection Copy(IniFile destinationFile) { return new IniSection(destinationFile, this); }
        
        /// <summary>
        /// Serializes the specified object into this <see cref="IniSection"/>.
        /// </summary>
        /// <typeparam name="T">The type of serialized object.</typeparam>
        /// <param name="source">The object to serialize.</param>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#serializing" target="_self">IniSection's Object Serialization</seealso>
        public void Serialize<T>(T source) where T : class, new() { IniSerializer.Serialize(source, this); }

        /// <summary>
        /// Deserializes this <see cref="IniSection"/> into an object of specified type.
        /// </summary>
        /// <typeparam name="T">The type of deserialized object.</typeparam>
        /// <returns>The object being deserialized.</returns>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#serializing" target="_self">IniSection's Object Serialization</seealso>
        public T Deserialize<T>() where T : class, new() { return IniSerializer.Deserialize<T>(this); }

        private static IEnumerable<IniKey> GetIniKeysFromKeyValuePairs(IniFile parentFile, IEnumerable<KeyValuePair<string, string>> nameValuePairs)
        {
            if (nameValuePairs != null)
                foreach (var pair in nameValuePairs)
                    yield return new IniKey(parentFile, pair);
        }
    }
}