using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using MadMilkman.Ini;

namespace MadMilkman.Ini.Samples.CS
{
    class IniSamples
    {
        private static void HelloWorld()
        {
            // Create new file.
            IniFile file = new IniFile();

            // Add new section.
            IniSection section = file.Sections.Add("Section Name");

            // Add new key and its value.
            IniKey key = section.Keys.Add("Key Name", "Hello World");

            // Read file's specific value.
            Console.WriteLine(file.Sections["Section Name"].Keys["Key Name"].Value);
        }

        private static void Create()
        {
            // Create new file with a default formatting.
            IniFile file = new IniFile(new IniOptions());

            // Add new content.
            IniSection section = new IniSection(file, IniSection.GlobalSectionName);
            IniKey key = new IniKey(file, "Key 1", "Value 1");
            file.Sections.Add(section);
            section.Keys.Add(key);

            // Add new content.
            file.Sections.Add("Section 2").Keys.Add("Key 2", "Value 2");
            
            // Add new content.
            file.Sections.Add(
                new IniSection(file, "Section 3",
                    new IniKey(file, "Key 3.1", "Value 3.1"),
                    new IniKey(file, "Key 3.2", "Value 3.2")));

            // Add new content.
            file.Sections.Add(
                new IniSection(file, "Section 4",
                    new Dictionary<string, string>()
                    {
                        {"Key 4.1", "Value 4.1"},
                        {"Key 4.2", "Value 4.2"}
                    }));
        }

        private static void Load()
        {
            IniOptions options = new IniOptions();
            IniFile iniFile = new IniFile(options);

            // Load file from path.
            iniFile.Load(@"..\..\..\MadMilkman.Ini.Samples.Files\Load Example.ini");

            // Load file from stream.
            using (Stream stream = File.OpenRead(@"..\..\..\MadMilkman.Ini.Samples.Files\Load Example.ini"))
                iniFile.Load(stream);

            // Load file's content from string.
            string iniContent = "[Section 1]" + Environment.NewLine +
                                "Key 1.1 = Value 1.1" + Environment.NewLine +
                                "Key 1.2 = Value 1.2" + Environment.NewLine +
                                "Key 1.3 = Value 1.3" + Environment.NewLine +
                                "Key 1.4 = Value 1.4";
            using (Stream stream = new MemoryStream(options.Encoding.GetBytes(iniContent)))
                iniFile.Load(stream);

            // Read file's content.
            foreach (var section in iniFile.Sections)
            {
                Console.WriteLine("SECTION: {0}", section.Name);
                foreach (var key in section.Keys)
                    Console.WriteLine("KEY: {0}, VALUE: {1}", key.Name, key.Value);
            }
        }

        private static void Style()
        {
            IniFile file = new IniFile();
            file.Sections.Add("Section 1").Keys.Add("Key 1", "Value 1");
            file.Sections.Add("Section 2").Keys.Add("Key 2", "Value 2");
            file.Sections.Add("Section 3").Keys.Add("Key 3", "Value 3");

            // Add leading comments.
            file.Sections[0].LeadingComment.Text = "Section 1 leading comment.";
            file.Sections[0].Keys[0].LeadingComment.Text = "Key 1 leading comment.";

            // Add trailing comments.
            file.Sections[1].TrailingComment.Text = "Section 2 trailing comment.";
            file.Sections[1].Keys[0].TrailingComment.Text = "Key 2 trailing comment.";

            // Add left space, indentation.
            file.Sections[1].LeftIndentation = 4;
            file.Sections[1].TrailingComment.LeftIndentation = 4;
            file.Sections[1].Keys[0].LeftIndentation = 4;
            file.Sections[1].Keys[0].TrailingComment.LeftIndentation = 4;

            // Add above space, empty lines.
            file.Sections[2].TrailingComment.EmptyLinesBefore = 2;
        }

        private static void Save()
        {
            IniOptions options = new IniOptions();
            IniFile iniFile = new IniFile(options);
            iniFile.Sections.Add(
                new IniSection(iniFile, "Section 1",
                    new IniKey(iniFile, "Key 1.1", "Value 1.1"),
                    new IniKey(iniFile, "Key 1.2", "Value 1.2"),
                    new IniKey(iniFile, "Key 1.3", "Value 1.3"),
                    new IniKey(iniFile, "Key 1.4", "Value 1.4")));

            // Save file to path.
            iniFile.Save(@"..\..\..\MadMilkman.Ini.Samples.Files\Save Example.ini");

            // Save file to stream.
            using (Stream stream = File.Create(@"..\..\..\MadMilkman.Ini.Samples.Files\Save Example.ini"))
                iniFile.Save(stream);

            // Save file's content to string.
            string iniContent;
            using (Stream stream = new MemoryStream())
            {
                iniFile.Save(stream);
                iniContent = new StreamReader(stream, options.Encoding).ReadToEnd();
            }

            Console.WriteLine(iniContent);
        }

        private static void Custom()
        {
            // Create new file with custom formatting.
            IniFile file = new IniFile(
                                new IniOptions()
                                {
                                    CommentStarter = IniCommentStarter.Hash,
                                    KeyDelimiter = IniKeyDelimiter.Colon,
                                    KeySpaceAroundDelimiter = true,
                                    SectionWrapper = IniSectionWrapper.CurlyBrackets,
                                    Encoding = Encoding.UTF8
                                });

            // Load file.
            file.Load(@"..\..\..\MadMilkman.Ini.Samples.Files\Custom Example Input.ini");

            // Change first section's fourth key's value.
            file.Sections[0].Keys[3].Value = "NEW VALUE";

            // Save file.
            file.Save(@"..\..\..\MadMilkman.Ini.Samples.Files\Custom Example Output.ini");
        }

        private static void Copy()
        {
            // Create new file.
            IniFile file = new IniFile();

            // Add new content.
            IniSection section = file.Sections.Add("Section");
            IniKey key = section.Keys.Add("Key");

            // Add duplicate section.
            file.Sections.Add(section.Copy());

            // Add duplicate key.
            section.Keys.Add(key.Copy());

            // Create new file.
            IniFile newFile = new IniFile(new IniOptions());

            // Import first file's section to second file.
            newFile.Sections.Add(section.Copy(newFile));
        }

        private static void Parse()
        {
            IniFile file = new IniFile();
            string content = "[Highest Score]" + Environment.NewLine +
                              "Name = John Doe" + Environment.NewLine +
                              "Score = 3200000" + Environment.NewLine +
                              "Date = 12/31/2010" + Environment.NewLine +
                              "Time = 11:59:59";
            using (Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(content)))
                file.Load(stream);

            IniSection scoreSection = file.Sections["Highest Score"];

            string playerName = scoreSection.Keys["Name"].Value;

            // Retrieve key's value as long.
            long playerScore;
            scoreSection.Keys["Score"].TryParseValue(out playerScore);

            // Retrieve key's value as DateTime.
            DateTime scoreDate;
            scoreSection.Keys["Date"].TryParseValue(out scoreDate);

            // Retrieve key's value as TimeSpan.
            TimeSpan gameTime;
            scoreSection.Keys["Time"].TryParseValue(out gameTime);
        }

        static void Main()
        {
            HelloWorld();

            Create();

            Load();

            Style();

            Save();

            Custom();

            Copy();

            Parse();
        }
    }
}
