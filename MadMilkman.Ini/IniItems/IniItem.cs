using System;
using System.Diagnostics;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a base class for INI content items, <see cref="IniSection"/> and <see cref="IniKey"/>.
    /// </summary>
    /// <remarks>
    /// <para>All INI items share the same content like <see cref="Name"/>, <see cref="LeadingComment"/> and <see cref="TrailingComment"/>.
    /// These properties are defined on an <see cref="IniItem"/> class, a base class for INI content items.</para>
    /// </remarks>
    [DebuggerDisplay("Name = {Name}")]
    public abstract class IniItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniFile parentFile;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IniComment leadingComment;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IniComment trailingComment;

        /// <summary>
        /// Gets and sets the name of the current <see cref="IniItem"/>.
        /// </summary>
        /// <remarks>
        /// When setting <see cref="IniItem.Name"/> the value is verified by the item's <see cref="IniDuplication"/> rule.
        /// </remarks>
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.ParentCollectionCore == null || ((IItemNameVerifier)this.ParentCollectionCore).VerifyItemName(value))
                    this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of whitespace characters before this <see cref="IniItem.Name">item's name</see>.
        /// </summary>
        public int LeftIndentation { get; set; }

        /// <summary>
        /// Gets the <see cref="IniComment"/> object that represents a comment that follows this <see cref="IniItem"/> on the same line.
        /// </summary>
        public IniComment LeadingComment
        {
            get
            {
                if (this.leadingComment == null)
                    this.leadingComment = new IniComment(IniCommentType.Leading);
                return this.leadingComment;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal bool HasLeadingComment { get { return this.leadingComment != null; } }

        /// <summary>
        /// Gets the <see cref="IniComment"/> object that represents a comments that occur before this <see cref="IniItem"/>.
        /// </summary>
        public IniComment TrailingComment
        {
            get
            {
                if (this.trailingComment == null)
                    this.trailingComment = new IniComment(IniCommentType.Trailing);
                return this.trailingComment;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal bool HasTrailingComment { get { return this.trailingComment != null; } }

        /// <summary>
        /// Gets the <see cref="IniFile"/> to which this <see cref="IniItem"/> belongs to.
        /// </summary>
        public IniFile ParentFile { get { return this.parentFile; } }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal object ParentCollectionCore  { get; set; }

        internal IniItem(IniFile parentFile, string name, IniComment trailingComment = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (parentFile == null)
                throw new ArgumentNullException("parentFile");

            this.name = name;
            this.parentFile = parentFile;
            this.trailingComment = trailingComment;
        }

        // Deep copy constructor.
        internal IniItem(IniFile parentFile, IniItem sourceItem)
        {
            if (parentFile == null)
                throw new ArgumentNullException("parentFile");

            this.name = sourceItem.name;
            this.parentFile = parentFile;
            if (sourceItem.HasLeadingComment)
                this.leadingComment = new IniComment(sourceItem.leadingComment);
            if (sourceItem.HasTrailingComment)
                this.trailingComment = new IniComment(sourceItem.trailingComment);
        }
    }
}
