using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a collection of mapped <see cref="IniKey.Value"/>s and their results, used in <see cref="O:MadMilkman.Ini.IniKey.TryParseValue{T}">IniKey.TryParseValue</see> methods.
    /// </summary>
    public sealed class IniValueMappings
    {
        private readonly IDictionary<string, object> mappings;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Predicate<Type> mappedTypeVerifier;

        internal IniValueMappings(Predicate<Type> mappedTypeVerifier)
        {
            this.mappings = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.mappedTypeVerifier = mappedTypeVerifier;
        }

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

            if (this.Contains(value) || !mappedTypeVerifier(typeof(T)))
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
