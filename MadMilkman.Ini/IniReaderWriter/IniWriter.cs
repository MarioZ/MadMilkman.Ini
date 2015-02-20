using System;
using System.IO;

namespace MadMilkman.Ini
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
     Justification = "StringWriter doesn't have unmanaged resources.")]
    internal sealed class IniWriter
    {
        private static readonly string[] NewLines = { "\r\n", "\n", "\r" };
        private readonly IniOptions options;
        private TextWriter writer;

        public IniWriter(IniOptions options) { this.options = options; }

        public void Write(IniFile iniFile, TextWriter textWriter)
        {
            this.writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            this.WriteSections(iniFile.Sections);

            textWriter.Write(this.EncryptAndCompressText(this.writer.ToString()));
            textWriter.Flush();
        }

        private string EncryptAndCompressText(string fileContent)
        {
            if (!string.IsNullOrEmpty(this.options.EncryptionPassword))
                fileContent = IniEncryptor.Encrypt(fileContent, this.options.EncryptionPassword, this.options.Encoding);

            if (this.options.Compression)
                fileContent = IniCompressor.Compress(fileContent, this.options.Encoding);

            return fileContent;
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
            this.WriteItem(section,
                // E.g. "   [SectionName]"
                new String(' ', section.LeftIndentation) +
                this.options.sectionWrapperStart +
                section.Name +
                this.options.sectionWrapperEnd);

            this.WriteKeys(section.Keys);
        }

        private void WriteKeys(IniKeyCollection keys)
        {
            foreach (IniKey key in keys)
                this.WriteItem(key,
                    // E.g. "   KeyName = KeyValue"
                    new String(' ', key.LeftIndentation) +
                    key.Name +
                    ((this.options.KeySpaceAroundDelimiter) ? " " : string.Empty) +
                    (char)this.options.KeyDelimiter +
                    ((this.options.KeySpaceAroundDelimiter) ? " " : string.Empty) +
                    key.Value);
        }

        private void WriteItem(IniItem item, string itemContent)
        {
            if (item.HasTrailingComment)
                this.WriteTrailingComment(item.TrailingComment);

            if (item.HasLeadingComment)
                this.WriteEmptyLines(item.LeadingComment.EmptyLinesBefore);

            this.writer.Write(itemContent);

            if (item.HasLeadingComment)
                this.WriteLeadingComment(item.LeadingComment);
            else
                this.writer.WriteLine();
        }

        private void WriteTrailingComment(IniComment trailingComment)
        {
            this.WriteEmptyLines(trailingComment.EmptyLinesBefore);

            // E.g. "   ;CommentText
            //          ;CommentText"
            if (trailingComment.Text != null)
                foreach (string commentLine in trailingComment.Text.Split(IniWriter.NewLines, StringSplitOptions.None))
                    this.writer.WriteLine(
                        new String(' ', trailingComment.LeftIndentation) +
                        (char)this.options.CommentStarter +
                        commentLine);
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

        private void WriteEmptyLines(int count)
        {
            for (int i = 0; i < count; i++)
                this.writer.WriteLine();
        }
    }
}