using System;
using System.IO;

namespace MadMilkman.Ini
{
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

        public void Read(IniFile iniFile, TextReader reader)
        {
            this.reader = reader;

            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                if (line.Trim().Length == 0)
                    this.currentEmptyLinesBefore++;
                else
                    this.ReadLine(line, iniFile);
            }
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

        private void ReadSection(int leftIndention, string line, IniFile file)
        {
            /* REMARKS:  First occurrence of section's end wrapper character (e.g. ']') defines section's name.
             *           The rest of the text is processed as leading comment or ignored.
             *          
             * CONSIDER: Implement a support for section's name which contains end wrapper characters. */

            int sectionEndIndex = line.IndexOf(this.options.sectionWrapperEnd, leftIndention);
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

                var currentKey = new IniKey(file,
                                            line.Substring(leftIndention, keyDelimiterIndex - leftIndention).TrimEnd(),
                                            this.currentTrailingComment)
                                            {
                                                LeftIndentation = leftIndention,
                                                LeadingComment = { EmptyLinesBefore = this.currentEmptyLinesBefore }
                                            };

                this.currentSection.Keys.Add(currentKey);

                this.ReadKeyValueAndLeadingComment(line.Substring(++keyDelimiterIndex).TrimStart(), currentKey);
            }

            this.currentTrailingComment = null;
        }

        private void ReadKeyValueAndLeadingComment(string lineLeftover, IniKey key)
        {
            /* REMARKS:  First occurrence of comment's starting character (e.g. ';') defines key's value.
             *          
             * CONSIDER: Implement a support for quoted values, thus enabling them to contain comment's starting characters. */

            int valueEndIndex = lineLeftover.IndexOf((char)this.options.CommentStarter);

            if (valueEndIndex == -1)
                key.Value = lineLeftover.TrimEnd();

            else if (valueEndIndex == 0)
                key.Value = key.LeadingComment.Text = string.Empty;

            else
            {
                key.LeadingComment.Text = lineLeftover.Substring(valueEndIndex + 1);

                // The amount of 'whitespace' characters between key's value and comment's starting character.
                int leftIndention = 0;
                while (lineLeftover[--valueEndIndex] == ' ' || lineLeftover[valueEndIndex] == '\t')
                    leftIndention++;

                key.LeadingComment.LeftIndentation = leftIndention;
                key.Value = lineLeftover.Substring(0, ++valueEndIndex);
            }
        }
    }
}