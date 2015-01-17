namespace MadMilkman.Ini
{
    /// <summary>
    /// Defines <see cref="IniSection">section's</see> wrapper characters around its name.
    /// </summary>
    public enum IniSectionWrapper
    {
        /// <summary>
        /// Use '[' and ']' as <see cref="IniSection">section's</see> wrapper characters.
        /// </summary>
        SquareBrackets = 0,
        /// <summary>
        /// Use '&lt;' and '&gt;' as <see cref="IniSection">section's</see> wrapper characters.
        /// </summary>
        AngleBrackets,
        /// <summary>
        /// Use '{' and '}' as <see cref="IniSection">section's</see> wrapper characters.
        /// </summary>
        CurlyBrackets,
        /// <summary>
        /// Use '(' and ')' as <see cref="IniSection">section's</see> wrapper characters.
        /// </summary>
        Parentheses
    }
}

