using System;
using System.Diagnostics;
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
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#parsing" target="_self">IniKey's Value Parsing</seealso>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#binding" target="_self">IniKey's Value Binding</seealso>
        public string Value { get; set; }

        /// <summary>
        /// Gets the <see cref="IniKeyCollection"/> to which this <see cref="IniKey"/> belongs to.
        /// </summary>
        public IniKeyCollection ParentCollection { get { return (IniKeyCollection)this.ParentCollectionCore; } }

        /// <summary>
        /// Gets the <see cref="IniSection"/> to which this <see cref="IniKey"/> belongs to.
        /// </summary>
        public IniSection ParentSection { get { return (IniSection)((this.ParentCollectionCore != null) ? this.ParentCollection.Owner : null); } }

        internal bool IsValueArray
        {
            get
            {
                return !string.IsNullOrEmpty(this.Value) && this.Value[0] == '{' && this.Value[this.Value.Length - 1] == '}';
            }
        }

        internal string[] Values
        {
            get
            {
                var values = this.Value.Substring(1, this.Value.Length - 2).Split(',');
                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Trim();
                return values;
            }
            set
            {
                this.Value = "{" + string.Join(",", value) + "}";
            }
        }

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
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#copying" target="_self">IniItem's Copying</seealso>
        public IniKey Copy() { return this.Copy(this.ParentFile); }

        /// <summary>
        /// Copies this <see cref="IniKey"/> instance and sets copied instance's <see cref="IniItem.ParentFile">ParentFile</see>.
        /// </summary>
        /// <param name="destinationFile">Copied key's parent file.</param>
        /// <returns>Copied <see cref="IniKey"/> that belongs to a specified <see cref="IniFile"/>.</returns>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#copying" target="_self">IniItem's Copying</seealso>
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
        /// <item><description>System.Decimal</description></item>
        /// <item><description>System.DateTime</description></item>
        /// <item><description>System.TimeSpan</description></item>
        /// <item><description>System.Enum</description></item>
        /// <item><description>System.String</description></item>
        /// </list>
        /// Additionally both Array and List of the above types are supported.
        /// </remarks>
        public static bool IsSupportedValueType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(String) ||
                   type == typeof(Decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(TimeSpan) ||
                   (type.IsArray && IsSupportedValueType(type.GetElementType())) ||
                   (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && IsSupportedValueType(type.GetGenericArguments()[0]));
        }

        /// <summary>
        /// Converts the <see cref="IniKey.Value"/> to an instance of the specified type.
        /// </summary>
        /// <param name="result">Uninitialized instance of a specific type which will hold the converted value if the conversion succeeds.</param>
        /// <typeparam name="T">Type of the object to convert the <see cref="IniKey.Value"/> to.</typeparam>
        /// <returns>Value that indicates whether the conversion succeeded.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="TryParseValueSupport"]/*'/>
        public bool TryParseValue<T>(out T result)
        {
            if (this.ParentFile.HasValueMappings && this.ParentFile.ValueMappings.TryGetResult<T>(this.Value, out result))
                return true;
            else
                return IniValueParser<T>.TryParse(this.Value, out result);
        }

        /// <summary>
        /// Converts the <see cref="IniKey.Value"/> to an array of the specified type.
        /// </summary>
        /// <param name="results">Uninitialized array of a specific type which will hold the converted values if the conversion succeeds.</param>
        /// <typeparam name="T">Type of the objects in array to convert the <see cref="IniKey.Value"/> to.</typeparam>
        /// <returns>Value that indicates whether the conversion succeeded.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="TryParseValueSupport"]/*'/>
        public bool TryParseValue<T>(out T[] results)
        {
            List<T> listResults;
            if (this.TryParseValue<T>(out listResults))
            {
                results = listResults.ToArray();
                return true;
            }

            results = null;
            return false;
        }

        /// <summary>
        /// Converts the <see cref="IniKey.Value"/> to a list of the specified type.
        /// </summary>
        /// <param name="results">Uninitialized list of a specific type which will hold the converted values if the conversion succeeds.</param>
        /// <typeparam name="T">Type of the objects in list to convert the <see cref="IniKey.Value"/> to.</typeparam>
        /// <returns>Value that indicates whether the conversion succeeded.</returns>
        /// <include file='SharedDocumentationComments.xml' path='Comments/Comment[@name="TryParseValueSupport"]/*'/>
        public bool TryParseValue<T>(out List<T> results)
        {
            if (this.IsValueArray)
            {
                var listResults = new List<T>();
                foreach (var value in this.Values)
                {
                    T result;
                    if (this.ParentFile.HasValueMappings && this.ParentFile.ValueMappings.TryGetResult<T>(value, out result))
                        listResults.Add(result);
                    else if (IniValueParser<T>.TryParse(value, out result))
                        listResults.Add(result);
                    else
                    {
                        results = null;
                        return false;
                    }
                }

                results = listResults;
                return true;
            }

            results = null;
            return false;
        }
    }
}