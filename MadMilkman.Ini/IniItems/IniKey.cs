namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a key item of the INI file with name and value content.
    /// </summary>
    public sealed class IniKey : IniItem
    {
        /* REMARKS:  Nicer implementation of Value property could be to define a new type, IniValue or IniString class.
         *           We could implement implicit casting to/from string and explicit casting to any type that we wish to support.
         * 
         * CONSIDER: Explicit casting often leads to confusion, also this feels like an inappropriate usage of it.
         *           My guess is that this could potentially lead to misinterpretation and thus create an unexpected behaviour. */

        /// <summary>
        /// Gets and sets <see cref="IniKey"/> value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets the <see cref="IniKeyCollection"/> to which this <see cref="IniKey"/> belongs to.
        /// </summary>
        public IniKeyCollection ParentCollection { get { return (IniKeyCollection)this.ParentCollectionCore; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniKey"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="name">The key's name.</param>
        public IniKey(IniFile parentFile, string name) : this(parentFile, name, (string)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniKey"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="name">The key's name.</param>
        /// <param name="value">The key's value.</param>
        public IniKey(IniFile parentFile, string name, string value) : base(parentFile, name) { this.Value = value; }

        // Constructor used by IniReader.
        internal IniKey(IniFile parentFile, string name, IniComment trailingComment)
            : base(parentFile, name, trailingComment) { }

        // Deep copy constructor.
        internal IniKey(IniFile destinationFile, IniKey sourceKey)
            : base(destinationFile, sourceKey) { this.Value = sourceKey.Value; }

        /// <summary>
        /// Copies this <see cref="IniKey"/> instance.
        /// </summary>
        /// <returns>Copied <see cref="IniKey"/>.</returns>
        public IniKey Copy() { return this.Copy(this.ParentFile); }

        /// <summary>
        /// Copies this <see cref="IniKey"/> instance and sets copied instance's <see cref="IniItem.ParentFile">ParentFile</see>.
        /// </summary>
        /// <param name="destinationFile">Copied key's parent file.</param>
        /// <returns>Copied <see cref="IniKey"/> that belongs to a specified <see cref="IniFile"/>.</returns>
        public IniKey Copy(IniFile destinationFile) { return new IniKey(destinationFile, this); }
    }
}