using System;
using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a key item of the INI file with name and value content.
    /// </summary>
    public sealed class IniKey : IniItem
    {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="IniKey"/> class.
        /// </summary>
        /// <param name="parentFile">The owner file.</param>
        /// <param name="nameValuePair">The key's data, pair of key's name and key's value.</param>
        public IniKey(IniFile parentFile, KeyValuePair<string, string> nameValuePair) : base(parentFile, nameValuePair.Key) { this.Value = nameValuePair.Value; }

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

        /// <summary>
        /// Indicates whether the <see cref="IniKey.Value"/> can be converted to specified type.
        /// </summary>
        /// <param name="type">Type of the object to convert the <see cref="IniKey.Value"/> to.</param>
        /// <returns><see langword="true"/> if the specified type is supported.</returns>
        /// <remarks>
        /// Currently supported types are:
        /// <list type="bullet">
        /// <item><description>System.Boolean</description></item>
        /// <item><description>System.Byte</description></item>
        /// <item><description>System.SByte</description></item>
        /// <item><description>System.Int16</description></item>
        /// <item><description>System.UInt16</description></item>
        /// <item><description>System.Int32</description></item>
        /// <item><description>System.UInt32</description></item>
        /// <item><description>System.Int64</description></item>
        /// <item><description>System.UInt64</description></item>
        /// <item><description>System.Char</description></item>
        /// <item><description>System.Single</description></item>
        /// <item><description>System.Double</description></item>
        /// <item><description>System.DateTime</description></item>
        /// <item><description>System.TimeSpan</description></item>
        /// </list>
        /// </remarks>
        public static bool IsSupportedValueType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.IsPrimitive || type.Equals(typeof(DateTime)) || type.Equals(typeof(TimeSpan));
        }

        /// <summary>
        /// Converts the <see cref="IniKey.Value"/> to an instance of the specified type.
        /// </summary>
        /// <param name="result">Uninitialized instance of a specific type which will hold the converted value if the conversion succeeds.</param>
        /// <typeparam name="T">Type of the object to convert the <see cref="IniKey.Value"/> to.</typeparam>
        /// <returns>Value that indicates whether the conversion succeeded.</returns>
        /// <remarks>For supported types see the remarks of <see cref="IsSupportedValueType(Type)"/> method.</remarks>
        public bool TryParseValue<T>(out T result)
        {
            var type = typeof(T);

            if (!IsSupportedValueType(type))
                throw new NotSupportedException();

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
            if (converter.IsValid(this.Value))
            {
                result = (T)converter.ConvertFromString(this.Value);
                return true;
            }

            result = default(T);
            return false;
        }
    }
}