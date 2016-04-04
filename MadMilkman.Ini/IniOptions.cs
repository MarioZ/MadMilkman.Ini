#region License & Copyright Notice
/* Copyright 2015 Mario Zorica.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */
#endregion

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
    /// <term><see cref="IniOptions.CommentStarter">Compression</see></term>
    /// <description><see langword="false"/></description>
    /// </item>
    /// <item>
    /// <term><see cref="IniOptions.CommentStarter">EncryptionPassword</see></term>
    /// <description><see langword="null"/></description>
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IniSectionWrapper sectionWrapper;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal char sectionWrapperStart;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal char sectionWrapperEnd;

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
        /// <para>Gets or sets a value indicating if file's size is reduced.</para>
        /// <para>If <see langword="true"/> file is decompressed on Load and compressed on Save.</para>
        /// </summary>
        public bool Compression { get; set; }

        /// <summary>
        /// <para>Gets or sets an INI file's protection password.</para>
        /// <para>File is decrypted on Load and encrypted on Save if a password is not <see langword="null"/> or <see cref="System.String.Empty"/>.</para>
        /// </summary>
        public string EncryptionPassword { get; set; }

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
        public IniSectionWrapper SectionWrapper
        {
            get { return this.sectionWrapper; }
            set
            {
                this.sectionWrapper = value;
                this.SectionWrapperToCharacters(out this.sectionWrapperStart, out this.sectionWrapperEnd);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniOptions"/> class.
        /// </summary>
        public IniOptions()
        {
            this.encoding = Encoding.ASCII;
            this.CommentStarter = IniCommentStarter.Semicolon;
            this.Compression = false;
            this.EncryptionPassword = null;
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
            this.encoding = options.encoding;
            this.CommentStarter = options.CommentStarter;
            this.Compression = options.Compression;
            this.EncryptionPassword = options.EncryptionPassword;
            this.KeyDelimiter = options.KeyDelimiter;
            this.KeyDuplicate = options.KeyDuplicate;
            this.KeyNameCaseSensitive = options.KeyNameCaseSensitive;
            this.KeySpaceAroundDelimiter = options.KeySpaceAroundDelimiter;
            this.SectionDuplicate = options.SectionDuplicate;
            this.SectionNameCaseSensitive = options.SectionNameCaseSensitive;
            this.SectionWrapper = options.SectionWrapper;
        }

        /* REMARKS: ToChar(bool) extension method on IniSectionWrapper would be nice, but in order to define
         *          an extension method in .NET 2.0 we need to declare ExtensionAttribute our self.
         *          
         *          Changing SectionWrapperToCharacters method's return type to Tuple<char, char> would be nice,
         *          but writing our own Tuple implementation for .NET 2.0 is an unnecessary overhead. */

        private void SectionWrapperToCharacters(out char startCharacter, out char endCharacte)
        {
            switch (this.sectionWrapper)
            {
                case IniSectionWrapper.AngleBrackets:
                    startCharacter = '<';
                    endCharacte = '>';
                    break;
                case IniSectionWrapper.CurlyBrackets:
                    startCharacter = '{';
                    endCharacte = '}';
                    break;
                case IniSectionWrapper.Parentheses:
                    startCharacter = '(';
                    endCharacte = ')';
                    break;
                default:
                    startCharacter = '[';
                    endCharacte = ']';
                    break;
            }
        }
    }
}
