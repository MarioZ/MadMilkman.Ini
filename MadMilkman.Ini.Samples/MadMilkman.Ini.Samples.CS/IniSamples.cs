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
            string content = "[Player]" + Environment.NewLine +
                             "Full Name = John Doe" + Environment.NewLine +
                             "Birthday = 12/31/1999" + Environment.NewLine +
                             "Married = Yes" + Environment.NewLine +
                             "Score = 9999999" + Environment.NewLine +
                             "Game Time = 00:59:59";
            using (Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(content)))
                file.Load(stream);

            // Map 'yes' value as 'true' boolean.
            file.ValueMappings.Add("yes", true);
            // Map 'no' value as 'false' boolean.
            file.ValueMappings.Add("no", false);

            IniSection playerSection = file.Sections["Player"];

            // Retrieve player's name.
            string playerName = playerSection.Keys["Full Name"].Value;

            // Retrieve player's birthday as DateTime.
            DateTime playerBirthday;
            playerSection.Keys["Birthday"].TryParseValue(out playerBirthday);

            // Retrieve player's marital status as bool.
            // TryParseValue succeeds due to the mapping of 'yes' value to 'true' boolean.
            bool playerMarried;
            playerSection.Keys["Married"].TryParseValue(out playerMarried);

            // Retrieve player's score as long.
            long playerScore;
            playerSection.Keys["Score"].TryParseValue(out playerScore);

            // Retrieve player's game time as TimeSpan.
            TimeSpan playerGameTime;
            playerSection.Keys["Game Time"].TryParseValue(out playerGameTime);
        }

        private static void BindInternal()
        {
            IniFile file = new IniFile();
            string content = "[Machine Settings]" + Environment.NewLine +
                             "Program Files = C:\\Program Files" + Environment.NewLine +
                             "[Application Settings]" + Environment.NewLine +
                             "Name = Example App" + Environment.NewLine +
                             "Version = 1.0" + Environment.NewLine +
                             "Full Name = @{Name} v@{Version}" + Environment.NewLine +
                             "Executable Path = @{Machine Settings|Program Files}\\@{Name}.exe";
            using (Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(content)))
                file.Load(stream);

            // Bind placeholders with file's content, internal information.
            file.ValueBinding.Bind();

            // Retrieve application's full name, value is 'Example App v1.0'.
            string appFullName = file.Sections["Application Settings"].Keys["Full Name"].Value;

            // Retrieve application's executable path, value is 'C:\\Program Files\\Example App.exe'.
            string appExePath = file.Sections["Application Settings"].Keys["Executable Path"].Value;
        }

        private static void BindExternal()
        {
            IniFile file = new IniFile();
            string content = "[User's Settings]" + Environment.NewLine +
                             "Nickname = @{User Alias}" + Environment.NewLine +
                             "Full Name = @{User Name} @{User Surname}" + Environment.NewLine +
                             "Profile Page = @{Homepage}/Profiles/@{User Alias}";
            using (Stream stream = new MemoryStream(Encoding.ASCII.GetBytes(content)))
                file.Load(stream);

            // Bind placeholders with user's data, external information.
            file.ValueBinding.Bind(
                new Dictionary<string, string>
                {
                    {"User Alias", "Johny"},
                    {"User Name", "John"},
                    {"User Surname", "Doe"}
                });

            // Bind 'Homepage' placeholder with 'www.example.com' value.
            file.ValueBinding.Bind(
                new KeyValuePair<string, string>("Homepage", "www.example.com"));

            // Retrieve user's full name, value is 'John Doe'.
            string userFullName = file.Sections["User's Settings"].Keys["Full Name"].Value;

            // Retrieve user's profile page, value is 'www.example.com/Profiles/Johny'.
            string userProfilePage = file.Sections["User's Settings"].Keys["Profile Page"].Value;
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

            BindInternal();

            BindExternal();
        }
    }
}
