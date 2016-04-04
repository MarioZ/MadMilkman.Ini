using System;
using System.IO;

namespace MadMilkman.Ini
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
     Justification = "StringReader doesn't have unmanaged resources.")]
    internal sealed class IniReader
    {
        private readonly IniOptions options;
        private TextReader reader;

        private int currentEmptyLinesBefore;
        private IniComment currentTrailingComment;
        private IniSection currentSection;

        public IniReader(IniOptions options)
        {
            this.options = options;
            this.currentEmptyLinesBefore = 0;
            this.currentTrailingComment = null;
            this.currentSection = null;
        }

        public void Read(IniFile iniFile, TextReader textReader)
        {
            this.reader = new StringReader(this.DecompressAndDecryptText(textReader.ReadToEnd()));

            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                if (line.Trim().Length == 0)
                    this.currentEmptyLinesBefore++;
                else
                    this.ReadLine(line, iniFile);
            }
        }

        private string DecompressAndDecryptText(string fileContent)
        {
            if (this.options.Compression)
                fileContent = IniCompressor.Decompress(fileContent, this.options.Encoding);

            if (!string.IsNullOrEmpty(this.options.EncryptionPassword))
                fileContent = IniEncryptor.Decrypt(fileContent, this.options.EncryptionPassword, this.options.Encoding);

            return fileContent;
        }

        private void ReadLine(string line, IniFile file)
        {
            /* REMARKS:  All 'whitespace' and 'tab' characters increase the LeftIndention by 1.
             *          
             * CONSIDER: Implement different processing of 'tab' characters. They are often represented as 4 spaces,
             *           or they can stretch to a next 'tab stop' position which occurs each 8 characters:
             *           0       8       16  
             *           |.......|.......|... */

            // Index of first non 'whitespace' character.
            int startIndex = Array.FindIndex(line.ToCharArray(), c => !(char.IsWhiteSpace(c) || c == '\t'));
            char startCharacter = line[startIndex];

            if (startCharacter == (char)this.options.CommentStarter)
                this.ReadTrailingComment(startIndex, line.Substring(++startIndex));

            else if (startCharacter == this.options.sectionWrapperStart)
                this.ReadSection(startIndex, line, file);

            else
                this.ReadKey(startIndex, line, file);

            this.currentEmptyLinesBefore = 0;
        }

        private void ReadTrailingComment(int leftIndention, string text)
        {
            if (this.currentTrailingComment == null)
                this.currentTrailingComment = new IniComment(IniCommentType.Trailing)
                {
                    EmptyLinesBefore = this.currentEmptyLinesBefore,
                    LeftIndentation = leftIndention,
                    Text = text
                };
            else
                this.currentTrailingComment.Text += Environment.NewLine + text;
        }

        /* MZ(2015-08-29): Added support for section names that may contain end wrapper or comment starter characters. */
        private void ReadSection(int leftIndention, string line, IniFile file)
        {
            int sectionEndIndex = -1, potentialCommentIndex, tempIndex = leftIndention;
            while (tempIndex != -1 && ++tempIndex <= line.Length)
            {
                potentialCommentIndex = line.IndexOf((char)this.options.CommentStarter, tempIndex);

                if (potentialCommentIndex != -1)
                    sectionEndIndex = line.LastIndexOf(this.options.sectionWrapperEnd, potentialCommentIndex - 1, potentialCommentIndex - tempIndex);
                else
                    sectionEndIndex = line.LastIndexOf(this.options.sectionWrapperEnd, line.Length - 1, line.Length - tempIndex);

                if (sectionEndIndex != -1)
                    break;
                else
                    tempIndex = potentialCommentIndex;
            }

            if (sectionEndIndex != -1)
            {
                this.currentSection = new IniSection(file,
                                                     line.Substring(leftIndention + 1, sectionEndIndex - leftIndention - 1),
                                                     this.currentTrailingComment)
                                      {
                                          LeftIndentation = leftIndention,
                                          LeadingComment = { EmptyLinesBefore = this.currentEmptyLinesBefore }
                                      };
                file.Sections.Add(this.currentSection);

                if (++sectionEndIndex < line.Length)
                    this.ReadSectionLeadingComment(line.Substring(sectionEndIndex));
            }

            this.currentTrailingComment = null;
        }

        private void ReadSectionLeadingComment(string lineLeftover)
        {
            // Index of first non 'whitespace' character.
            int leftIndention = Array.FindIndex(lineLeftover.ToCharArray(), c => !(char.IsWhiteSpace(c) || c == '\t'));
            if (leftIndention != -1 && lineLeftover[leftIndention] == (char)this.options.CommentStarter)
            {
                var leadingComment = this.currentSection.LeadingComment;
                leadingComment.Text = lineLeftover.Substring(leftIndention + 1);
                leadingComment.LeftIndentation = leftIndention;
            }
        }

        private void ReadKey(int leftIndention, string line, IniFile file)
        {
            int keyDelimiterIndex = line.IndexOf((char)this.options.KeyDelimiter, leftIndention);
            if (keyDelimiterIndex != -1)
            {
                if (this.currentSection == null)
                    this.currentSection = file.Sections.Add(IniSection.GlobalSectionName);

                /* MZ(2016-04-04): Fixed issue with trimming values. */
                bool spacedDelimiter = keyDelimiterIndex > 0 && line[keyDelimiterIndex - 1] == ' ';
                string keyName = line.Substring(leftIndention, keyDelimiterIndex - leftIndention - (spacedDelimiter ? 1 : 0));
                var currentKey = new IniKey(file, keyName, this.currentTrailingComment)
                                 {
                                     LeftIndentation = leftIndention,
                                     LeadingComment = { EmptyLinesBefore = this.currentEmptyLinesBefore }
                                 };
                this.currentSection.Keys.Add(currentKey);

                ++keyDelimiterIndex;
                if (spacedDelimiter && keyDelimiterIndex < line.Length && line[keyDelimiterIndex] == ' ')
                    ++keyDelimiterIndex;

                this.ReadValue(line.Substring(keyDelimiterIndex), currentKey);
            }

            this.currentTrailingComment = null;
        }

        private void ReadValue(string lineLeftover, IniKey key)
        {
            int valueEndIndex = lineLeftover.IndexOf((char)this.options.CommentStarter);

            /* MZ(2016-04-04): Fixed issue with trimming values. */
            if (valueEndIndex == -1)
                key.Value = lineLeftover;
            else if (valueEndIndex == 0)
            {
                key.Value = string.Empty;
                key.LeadingComment.Text = lineLeftover.Substring(1);
            }
            else
                this.ReadValueLeadingComment(lineLeftover, valueEndIndex, key);
        }

        /* MZ(2016-02-23): Added support for quoted values which can contain comment's starting characters. */
        private void ReadValueLeadingComment(string lineLeftover, int potentialCommentIndex, IniKey key)
        {
            int quoteEndIndex = lineLeftover.IndexOf('"', 1);
            if (lineLeftover[0] == '"' && quoteEndIndex != -1)
                while (quoteEndIndex > potentialCommentIndex && potentialCommentIndex != -1)
                    potentialCommentIndex = lineLeftover.IndexOf((char)this.options.CommentStarter, ++potentialCommentIndex);

            if (potentialCommentIndex == -1)
                key.Value = lineLeftover.TrimEnd();
            else
            {
                key.LeadingComment.Text = lineLeftover.Substring(potentialCommentIndex + 1);

                // The amount of 'whitespace' characters between key's value and comment's starting character.
                int leftIndention = 0;
                while (potentialCommentIndex > 0 && (lineLeftover[--potentialCommentIndex] == ' ' || lineLeftover[potentialCommentIndex] == '\t'))
                    leftIndention++;

                key.LeadingComment.LeftIndentation = leftIndention;
                key.Value = lineLeftover.Substring(0, ++potentialCommentIndex);
            }
        }
    }
}