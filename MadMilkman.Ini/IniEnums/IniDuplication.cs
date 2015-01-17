namespace MadMilkman.Ini
{
    /// <summary>
    /// Defines a behaviour for duplicate <see cref="IniItem.Name"/> occurences.
    /// </summary>
    public enum IniDuplication
    {
        /// <summary>
        /// Allow duplicate names in <see cref="IniItemCollection{T}"/>.
        /// </summary>
        Allowed = 0,
        /// <summary>
        /// Disallow duplicate names in <see cref="IniItemCollection{T}"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="System.InvalidOperationException"/> is thrown on duplicate name occurence.
        /// </remarks>
        Disallowed,
        /// <summary>
        /// Ignore duplicate names.
        /// </summary>
        /// <remarks>
        /// Prevents adding or inserting an <see cref="IniItem"/> if its name already exists in <see cref="IniItemCollection{T}"/>.
        /// </remarks>
        Ignored
    }
}
