namespace MadMilkman.Ini
{
    /// <summary>
    /// Defines <see cref="IniComment">comment's</see> starting character.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
     Justification = "I'm defining and using enum values with specific characters and null character, aka '\0', has no purpose here.")]
    public enum IniCommentStarter
    {
        /// <summary>
        /// Use ';' as <see cref="IniComment">comment's</see> starting character.
        /// </summary>
        Semicolon = ';',
        /// <summary>
        /// Use '#' as <see cref="IniComment">comment's</see> starting character.
        /// </summary>
        Hash = '#'
    }
}
