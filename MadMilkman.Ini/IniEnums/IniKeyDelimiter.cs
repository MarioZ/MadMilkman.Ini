namespace MadMilkman.Ini
{
    /// <summary>
    /// Defines <see cref="IniKey">key's</see> name and value delimiter character.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
     Justification = "I'm defining and using enum values with specific characters and null character, aka '\0', has no purpose here.")]
    public enum IniKeyDelimiter
    {
        /// <summary>
        /// Use '=' as <see cref="IniKey">key's</see> name and value delimiter character.
        /// </summary>
        Equal = '=',
        /// <summary>
        /// Use ':' as <see cref="IniKey">key's</see> name and value delimiter character.
        /// </summary>
        Colon = ':'
    }
}
