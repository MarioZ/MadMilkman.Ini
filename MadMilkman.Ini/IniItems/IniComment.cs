using System.Diagnostics;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a comment object used by <see cref="IniItem"/> objects, <see cref="IniSection"/> and <see cref="IniKey"/>.
    /// </summary>
    [DebuggerDisplay("Text = {Text}")]
    public sealed class IniComment
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string text;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniCommentType type;

        /// <summary>
        /// Gets or sets the amount of empty lines before this <see cref="IniComment.Text">comment's text</see>.
        /// </summary>
        public int EmptyLinesBefore { get; set; }

        /// <summary>
        /// Gets or sets the amount of whitespace characters before this <see cref="IniComment.Text">comment's text</see>.
        /// </summary>
        public int LeftIndentation { get; set; }

        /// <summary>
        /// Gets or sets a text of this <see cref="IniComment"/> instance.
        /// </summary>
        /// <remarks>
        /// <para>For <see cref="IniItem.LeadingComment">LeadingComment</see> text should not contain new line characters.
        /// If it does, they will be replaced with a space characters.</para>
        /// </remarks>
        public string Text
        {
            get { return this.text; }
            set
            {
                if (value != null && this.type == IniCommentType.Leading)
                    this.text = value.Replace("\r\n", " ")
                                     .Replace("\n", " ")
                                     .Replace("\r", " ");
                else
                    this.text = value;
            }
        }

        internal IniComment(IniCommentType type) { this.type = type; }

        // Deep copy constructor.
        internal IniComment(IniComment source)
        {
            this.text = source.text;
            this.type = source.type;
            this.EmptyLinesBefore = source.EmptyLinesBefore;
            this.LeftIndentation = source.LeftIndentation;
        }
    }
}
