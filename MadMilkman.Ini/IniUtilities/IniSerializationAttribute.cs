using System;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Indicates the behavior of public property when serializing or deserializing the object that contains it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IniSerializationAttribute : Attribute
    {
        /// <summary>
        /// Gets the <see cref="IniKey"/> name of serialized the property.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the value indicating whether serialization is ignored.
        /// </summary>
        public bool Ignore { get; private set; }

        /// <summary>
        /// Initializes a new instance of the IniSerializationAttribute class and specifies the <see cref="IniKey"/>'s name.
        /// </summary>
        /// <param name="alias">The name of the generated <see cref="IniKey"/>.</param>
        public IniSerializationAttribute(string alias) { this.Alias = alias; }

        /// <summary>
        /// Initializes a new instance of the IniSerializationAttribute class and specifies if serialization is ignored.
        /// </summary>
        /// <param name="ignore">The value indicating whether serialization is ignored.</param>
        public IniSerializationAttribute(bool ignore) { this.Ignore = ignore; }

    }
}
