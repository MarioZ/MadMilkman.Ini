using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a class that is used for binding operations, an operation in which the <see cref="IniKey">placeholder keys</see> values are replaced with an internal or external data.
    /// </summary>
    /// <remarks>
    /// <para><see cref="IniValueBinding"/> can be accessed through <see cref="IniFile.ValueBinding"/> property.</para>
    /// <para>Binding can be executed with internal data source or with a provided external data source.</para>
    /// <para>For more information see <see href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#binding" target="_self">IniKey's Value Binding</see>.</para>
    /// </remarks>
    /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#binding" target="_self">IniKey's Value Binding</seealso>
    public sealed class IniValueBinding
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniValueBindingEventArgs args;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniFile iniFile;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Regex placeholderPattern = new Regex(@"@\{[\w\s|]+\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Occurs when a placeholder is binding with data source value and can be used to customize the binding operation.
        /// </summary>
        public event EventHandler<IniValueBindingEventArgs> Binding;

        internal IniValueBinding(IniFile iniFile)
        {
            if (iniFile == null)
                throw new ArgumentNullException("iniFile");

            this.iniFile = iniFile;
            this.args = new IniValueBindingEventArgs();
        }

        /// <summary>
        /// Executes a binding operation with internal data source.
        /// </summary>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#binding" target="_self">IniKey's Value Binding</seealso>
        public void Bind()
        {
            foreach (var placeholderPair in this.GetPlaceholderPairs(null))
            {
                IniKey placeholderKey = placeholderPair.Key;
                string placeholder = placeholderPair.Value;
                string placeholderName = placeholder.Substring(2, placeholder.Length - 3);
                string targetedValue;

                int separator = placeholder.IndexOf('|');
                if (separator != -1)
                {
                    var targetedSection = this.iniFile.Sections[placeholder.Substring(2, separator - 2)];
                    if (targetedSection == null)
                        continue;

                    targetedValue = GetTargetedValue(
                                        targetedSection,
                                        placeholder.Substring(separator + 1, placeholder.Length - separator - 2));
                }
                else
                    targetedValue = GetTargetedValue(
                                        placeholderKey.ParentSection,
                                        placeholderName);

                this.ExecuteBinding(placeholder, placeholderName, placeholderKey, targetedValue);
            }
        }

        /// <summary>
        /// Executes a binding operation with external data source.
        /// </summary>
        /// <param name="dataSource">The binding data source.</param>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#binding" target="_self">IniKey's Value Binding</seealso>
        public void Bind(object dataSource) { this.Bind(dataSource, null); }

        /// <summary>
        /// Executes a binding operation with external data source, only on specified section.
        /// </summary>
        /// <param name="dataSource">The binding data source.</param>
        /// <param name="sectionName">The <see cref="IniSection"/>'s name.</param>
        /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#binding" target="_self">IniKey's Value Binding</seealso>
        public void Bind(object dataSource, string sectionName)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            var dataSourceDictionary = CreateDataSourceDictionary(dataSource);
            if (dataSourceDictionary == null)
                return;

            foreach (var placeholderPair in this.GetPlaceholderPairs(this.iniFile.Sections[sectionName]))
            {
                IniKey placeholderKey = placeholderPair.Key;
                string placeholder = placeholderPair.Value;
                string placeholderName = placeholder.Substring(2, placeholder.Length - 3);
                string targetedValue;

                dataSourceDictionary.TryGetValue(placeholderName, out targetedValue);

                this.ExecuteBinding(placeholder, placeholderName, placeholderKey, targetedValue);
            }
        }

        private void ExecuteBinding(string placeholder, string placeholderName, IniKey placeholderKey, string targetedValue)
        {
            this.args.Initialize(placeholderName, placeholderKey, targetedValue, targetedValue != null);

            if (this.Binding != null)
                this.Binding(this, this.args);

            if (this.args.Value != null)
                placeholderKey.Value = placeholderKey.Value.Replace(placeholder, this.args.Value);

            this.args.Reset();
        }

        // Returns placeholder pairs as KeyValuePair<IniKey, string>:
        //       Key  =  IniKey in which value's the placeholder resides
        //     Value  =  Placeholder, e.g. @{Placeholder}
        private IEnumerable<KeyValuePair<IniKey, string>> GetPlaceholderPairs(IniSection section)
        {
            if (section != null)
            {
                foreach (IniKey key in section.Keys)
                    if (key.Value != null)
                    {
                        int matchStartat = key.Value.IndexOf("@{");
                        if (matchStartat != -1)
                            foreach (Match match in placeholderPattern.Matches(key.Value, matchStartat))
                                yield return new KeyValuePair<IniKey, string>(key, match.Value);
                    }
            }
            else
            {
                foreach (IniSection iniSection in this.iniFile.Sections)
                    foreach (var placeholderPair in this.GetPlaceholderPairs(iniSection))
                        yield return placeholderPair;
            }
        }

        private static string GetTargetedValue(IniSection targetedSection, string targetedKeyName)
        {
            IniKey targetedKey = targetedSection.Keys[targetedKeyName];
            return targetedKey != null ? targetedKey.Value : null;
        }

        /* REMARKS:  TODO re-factor this, use providers ...
         *          
         * CONSIDER: Implement support for any custom (including anonymous) types, use reflation ... */
        private static IDictionary<string, string> CreateDataSourceDictionary(object dataSource)
        {
            var dictionary = dataSource as IDictionary<string, string>;
            if (dictionary != null)
                return dictionary;

            var collection = dataSource as ICollection<KeyValuePair<string, string>>;
            if (collection != null)
            {
                dictionary = new Dictionary<string, string>(collection.Count);

                foreach (var dataSourceItem in collection)
                    if (!dictionary.ContainsKey(dataSourceItem.Key))
                        dictionary.Add(dataSourceItem);

                return dictionary;
            }

            var enumerator = dataSource as IEnumerable<KeyValuePair<string, string>>;
            if (enumerator != null)
            {
                dictionary = new Dictionary<string, string>();

                foreach (var dataSourceItem in enumerator)
                    if (!dictionary.ContainsKey(dataSourceItem.Key))
                        dictionary.Add(dataSourceItem);

                return dictionary;
            }

            if (dataSource is KeyValuePair<string, string>)
            {
                dictionary = new Dictionary<string, string>(1);
                dictionary.Add((KeyValuePair<string, string>)dataSource);
                return dictionary;
            }

            return null;
        }
    }
}
