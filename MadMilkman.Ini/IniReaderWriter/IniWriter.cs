using System;
using System.IO;

namespace MadMilkman.Ini
{
    internal sealed class IniWriter
    {
        private static readonly string[] NewLines = { "\r\n", "\n", "\r" };
        private readonly IniOptions options;
        private TextWriter writer;

        public IniWriter(IniOptions options) { this.options = options; }

        public void Write(IniFile iniFile, TextWriter writer)
        {
            this.writer = writer;
            this.WriteSections(iniFile.Sections);
            this.writer.Flush();
        }

        private void WriteSections(IniSectionCollection sections)
        {
            if (sections.Count == 0)
                return;

            this.WriteFirstSection(sections[0]);

            for (int i = 1; i < sections.Count; i++)
                this.WriteSection(sections[i]);
        }

        private void WriteFirstSection(IniSection section)
        {
            if (section.Name.Equals(IniSection.GlobalSectionName))
                this.WriteKeys(section.Keys);
            else
                this.WriteSection(section);
        }

        private void WriteSection(IniSection section)
        {
            this.WriteTrailingComment(section.TrailingComment, section.LeadingComment.EmptyLinesBefore);

            this.writer.Write(
                // E.g. "   [SectionName]"
                new String(' ', section.LeftIndentation) +
                IniSectionWrapperUtility.SectionWrapperToChar(this.options.SectionWrapper, true) +
                section.Name +
                IniSectionWrapperUtility.SectionWrapperToChar(this.options.SectionWrapper, false));

            this.WriteLeadingComment(section.LeadingComment);

            this.WriteKeys(section.Keys);
        }

        private void WriteKeys(IniKeyCollection keys)
        {
            foreach (IniKey key in keys)
                this.WriteKey(key);
        }

        private void WriteKey(IniKey key)
        {
            this.WriteTrailingComment(key.TrailingComment, key.LeadingComment.EmptyLinesBefore);

            this.writer.Write(
                // E.g. "   KeyName=KeyValue"
                new String(' ', key.LeftIndentation) +
                key.Name +
                ((this.options.KeySpaceAroundDelimiter) ? " " : string.Empty) +
                (char)this.options.KeyDelimiter +
                ((this.options.KeySpaceAroundDelimiter) ? " " : string.Empty) +
                key.Value);

            this.WriteLeadingComment(key.LeadingComment);
        }

        private void WriteTrailingComment(IniComment trailingComment, int leadingCommentEmptyLinesBefore)
        {
            for (int i = 0; i < trailingComment.EmptyLinesBefore; i++)
                this.writer.WriteLine();

            // E.g. "   ;CommentText
            //          ;CommentText"
            if (trailingComment.Text != null)
                foreach (string commentLine in trailingComment.Text.Split(IniWriter.NewLines, StringSplitOptions.None))
                    this.writer.WriteLine(
                        new String(' ', trailingComment.LeftIndentation) +
                        (char)this.options.CommentStarter +
                        commentLine);

            for (int i = 0; i < leadingCommentEmptyLinesBefore; i++)
                this.writer.WriteLine();
        }

        private void WriteLeadingComment(IniComment leadingComment)
        {
            // E.g. "   ;CommentText"
            if (leadingComment.Text != null)
                this.writer.WriteLine(
                    new String(' ', leadingComment.LeftIndentation) +
                    (char)this.options.CommentStarter +
                    leadingComment.Text);
            else
                this.writer.WriteLine();
        }
    }
}
