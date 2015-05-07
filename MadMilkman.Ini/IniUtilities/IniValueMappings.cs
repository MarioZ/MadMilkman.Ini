using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a class of mapped <see cref="IniKey.Value"/>s and their results, used in <see cref="O:MadMilkman.Ini.IniKey.TryParseValue"/> methods.
    /// </summary>
    /// <remarks>
    /// <para><see cref="IniValueMappings"/> can be accessed through <see cref="IniFile.ValueMappings"/> property.</para>
    /// <para>Mapped value results have priority over parsing the value.</para>
    /// <para>For more information see <see href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#parsing" target="_self">IniKey's Value Parsing</see>.</para>
    /// </remarks>
    /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#parsing" target="_self">IniKey's Value Parsing</seealso>
    public sealed class IniValueMappings
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Predicate<Type> MappedTypeVerifier = IniKey.IsSupportedValueType;

        private readonly IDictionary<string, object> mappings;

        internal IniValueMappings() { this.mappings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); }

        /// <summary>
        /// Adds a new mapping of <see cref="IniKey.Value"/> to resulting object of parse methods.
        /// </summary>
        /// <param name="value">The key's value.</param>
        /// <param name="mappedResult">The object that represents parsed <see cref="IniKey.Value"/>.</param>
        /// <typeparam name="T">Type of the object that represents parsed <see cref="IniKey.Value"/>.</typeparam>
        /// <remarks>
        /// <para>The key's value cannot be <see langword="null"/>.</para>
        /// <para>The mapped result's type must be one of the supported types for parsing, see the remarks of <see cref="IniKey.IsSupportedValueType(Type)"/> method.</para>
        /// <para>Collection cannot contain multiple entries of same key's value, value comparison is case-insensitive.</para>
        /// </remarks>
        public void Add<T>(string value, T mappedResult)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (this.Contains(value) || !IniValueMappings.MappedTypeVerifier(typeof(T)))
                throw new InvalidOperationException();

            this.mappings.Add(value, mappedResult);
        }

        /// <summary>
        /// Determines whether the collection contains a mapping for a specified key's value.
        /// </summary>
        /// <param name="value">The key's value to locate in the collection.</param>
        /// <returns><see langword="true"/> if the collection contains a mapping for a specified key's value.</returns>
        public bool Contains(string value) { return this.mappings.ContainsKey(value); }

        /// <summary>
        /// Removes a mapping for a specified key's value in the collection.
        /// </summary>
        /// <param name="value">The key's value to remove in the collection.</param>
        /// <returns><see langword="true"/> if a mapping for a specified key's value is successfully found and removed.</returns>
        public bool Remove(string value) { return this.mappings.Remove(value); }

        internal bool TryGetResult<T>(string value, out T result)
        {
            if (!string.IsNullOrEmpty(value))
            {
                object mappedResult;
                if (this.mappings.TryGetValue(value, out mappedResult) &&
                    mappedResult.GetType() == typeof(T))
                {
                    result = (T)mappedResult;
                    return true;
                }
            }
            result = default(T);
            return false;
        }
    }
}
