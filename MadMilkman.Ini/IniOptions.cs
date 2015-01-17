using System.Text;
using System.Diagnostics;

namespace MadMilkman.Ini
{
    /// <summary>
    /// Represents a class that defines INI file's format, stores properties used for both reading and writing a file.
    /// </summary>
    /// <remarks>
    /// <para>After an instance of this class is passed to an <see cref="IniFile"/> constructor, further changes on that instance's properties will have no effect.</para>
    /// <list type="table">
    /// <listheader>
    /// <term>Property</term>
    /// <description>Default Value</description>
    /// </listheader>
    /// <item>
    /// <term><see cref="IniOptions.CommentStarter">CommentStarter</see></term>
    /// <description><see cref="IniCommentStarter.Semicolon"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.Encoding">Encoding</see></term>
    /// <description><see cref="System.Text.Encoding.ASCII">Encoding.ASCII</see></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.KeyDelimiter">KeyDelimiter</see></term>
    /// <description><see cref="IniKeyDelimiter.Equal"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.KeyDuplicate">KeyDuplicate</see></term>
    /// <description><see cref="IniDuplication.Allowed"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.KeyNameCaseSensitive">KeyNameCaseSensitive</see></term>
    /// <description><see langword="false"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.KeySpaceAroundDelimiter">KeySpaceAroundDelimiter</see></term>
    /// <description><see langword="false"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.SectionDuplicate">SectionDuplicate</see></term>
    /// <description><see cref="IniDuplication.Allowed"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.SectionNameCaseSensitive">SectionNameCaseSensitive</see></term>
    /// <description><see langword="false"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.SectionWrapper">SectionWrapper</see></term>
    /// <description><see cref="IniSectionWrapper.SquareBrackets"/></description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class IniOptions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Encoding encoding;

        /// <summary>
        /// Gets or sets encoding for reading and writing an INI file.
        /// </summary>
        /// <remarks>
        /// Value should not be <see langword="null"/>, if it is then a default <see cref="System.Text.Encoding.ASCII">Encoding.ASCII</see> value will be used.
        /// </remarks>
        public Encoding Encoding
        {
            get { return this.encoding; }
            set
            {
                if (value == null)
                    this.encoding = Encoding.ASCII;
                this.encoding = value;
            }
        }

        /// <summary>
        /// Gets or sets comments starting character.
        /// </summary>
        public IniCommentStarter CommentStarter { get; set; }

        /// <summary>
        /// Gets or sets keys name and value delimiter character.
        /// </summary>
        public IniKeyDelimiter KeyDelimiter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether keys with same name are allowed, disallowed or ignored.
        /// </summary>
        public IniDuplication KeyDuplicate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether keys name are case sensitive.
        /// </summary>
        public bool KeyNameCaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether space is written around the keys delimiter.
        /// </summary>
        public bool KeySpaceAroundDelimiter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sections with same name are allowed, disallowed or ignored.
        /// </summary>
        public IniDuplication SectionDuplicate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sections name are case sensitive.
        /// </summary>
        public bool SectionNameCaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets wrapper characters of sections name.
        /// </summary>
        public IniSectionWrapper SectionWrapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniOptions"/> class.
        /// </summary>
        public IniOptions()
        {
            this.Encoding = Encoding.ASCII;
            this.CommentStarter = IniCommentStarter.Semicolon;
            this.KeyDelimiter = IniKeyDelimiter.Equal;
            this.KeyDuplicate = IniDuplication.Allowed;
            this.KeyNameCaseSensitive = false;
            this.KeySpaceAroundDelimiter = false;
            this.SectionDuplicate = IniDuplication.Allowed;
            this.SectionNameCaseSensitive = false;
            this.SectionWrapper = IniSectionWrapper.SquareBrackets;
        }

        // Deep copy constructor.
        internal IniOptions(IniOptions options)
        {
            this.Encoding = options.Encoding;
            this.CommentStarter = options.CommentStarter;
            this.KeyDelimiter = options.KeyDelimiter;
            this.KeyDuplicate = options.KeyDuplicate;
            this.KeyNameCaseSensitive = options.KeyNameCaseSensitive;
            this.KeySpaceAroundDelimiter = options.KeySpaceAroundDelimiter;
            this.SectionDuplicate = options.SectionDuplicate;
            this.SectionNameCaseSensitive = options.SectionNameCaseSensitive;
            this.SectionWrapper = options.SectionWrapper;
        }
    }
}
