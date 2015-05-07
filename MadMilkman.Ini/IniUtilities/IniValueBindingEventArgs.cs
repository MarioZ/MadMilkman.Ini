using System;
using System.Diagnostics;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Provides data for <see cref="IniValueBinding.Binding"/> event.
    /// </summary>
    public sealed class IniValueBindingEventArgs : EventArgs
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string placeholderName;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IniKey placeholderKey;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isValueFound;

        /// <summary>
        /// Gets the placeholder's name.
        /// </summary>
        public string PlaceholderName { get { return this.placeholderName; } }

        /// <summary>
        /// Gets the placeholder's <see cref="IniKey"/>.
        /// </summary>
        public IniKey PlaceholderKey { get { return this.placeholderKey; } }

        /// <summary>
        /// Gets or sets the data source value that will replace the placeholder.
        /// </summary>
        /// <value>
        /// The data source value that will replace the placeholder, if it's not <see langword="null"/>.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets a value indicating whether value was found in the data source.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if value was found in the data source.
        /// </value>
        public bool IsValueFound { get { return this.isValueFound; } }

        internal IniValueBindingEventArgs() { }

        internal void Initialize(string placeholderName, IniKey placeholderKey, string value, bool isValueFound)
        {
            this.placeholderName = placeholderName;
            this.placeholderKey = placeholderKey;
            this.Value = value;
            this.isValueFound = isValueFound;
        }

        internal void Reset() { this.Initialize(null, null, null, false); }
    }
}
