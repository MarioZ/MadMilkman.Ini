Imports System
Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports MadMilkman.Ini

Module IniSamples

    Private Sub HelloWorld()
        ' Create new file.
        Dim file As New IniFile()

        ' Add new section.
        Dim section As IniSection = file.Sections.Add("Section Name")

        ' Add new key and its value.
        Dim key As IniKey = section.Keys.Add("Key Name", "Hello World")

        ' Read file's specific value.
        Console.WriteLine(file.Sections("Section Name").Keys("Key Name").Value)
    End Sub

    Private Sub Create()
        ' Create new file with default formatting.
        Dim file As New IniFile(New IniOptions())

        ' Add new content.
        Dim section As New IniSection(file, IniSection.GlobalSectionName)
        Dim key As New IniKey(file, "Key 1", "Value 1")
        file.Sections.Add(section)
        section.Keys.Add(key)

        ' Add new content.
        file.Sections.Add("Section 2").Keys.Add("Key 2", "Value 2")

        ' Add new content.
        file.Sections.Add(
            New IniSection(file, "Section 3",
                New IniKey(file, "Key 3.1", "Value 3.1"),
                New IniKey(file, "Key 3.2", "Value 3.2")))

        ' Add new content.
        file.Sections.Add(
            New IniSection(file, "Section 4",
                New Dictionary(Of String, String)() From {
                    {"Key 4.1", "Value 4.1"},
                    {"Key 4.2", "Value 4.2"}
                }))
    End Sub

    Private Sub Load()
        Dim options As New IniOptions()
        Dim iniFile As New IniFile(options)

        ' Load file from path.
        iniFile.Load("..\..\..\MadMilkman.Ini.Samples.Files\Load Example.ini")

        ' Load file from stream.
        Using stream As Stream = File.OpenRead("..\..\..\MadMilkman.Ini.Samples.Files\Load Example.ini")
            iniFile.Load(stream)
        End Using

        ' Load file's content from string.
        Dim iniContent As String = "[Section 1]" + Environment.NewLine +
                                   "Key 1.1 = Value 1.1" + Environment.NewLine +
                                   "Key 1.2 = Value 1.2" + Environment.NewLine +
                                   "Key 1.3 = Value 1.3" + Environment.NewLine +
                                   "Key 1.4 = Value 1.4"
        iniFile.Load(New StringReader(iniContent))

        ' Read file's content.
        For Each section In iniFile.Sections
            Console.WriteLine("SECTION: {0}", section.Name)
            For Each key In section.Keys
                Console.WriteLine("KEY: {0}, VALUE: {1}", key.Name, key.Value)
            Next
        Next
    End Sub

    Private Sub Style()
        Dim file As New IniFile()
        file.Sections.Add("Section 1").Keys.Add("Key 1", "Value 1")
        file.Sections.Add("Section 2").Keys.Add("Key 2", "Value 2")
        file.Sections.Add("Section 3").Keys.Add("Key 3", "Value 3")

        ' Add leading comments.
        file.Sections(0).LeadingComment.Text = "Section 1 leading comment."
        file.Sections(0).Keys(0).LeadingComment.Text = "Key 1 leading comment."

        ' Add trailing comments.
        file.Sections(1).TrailingComment.Text = "Section 2 trailing comment."
        file.Sections(1).Keys(0).TrailingComment.Text = "Key 2 trailing comment."

        ' Add left space, indentation.
        file.Sections(1).LeftIndentation = 4
        file.Sections(1).TrailingComment.LeftIndentation = 4
        file.Sections(1).Keys(0).LeftIndentation = 4
        file.Sections(1).Keys(0).TrailingComment.LeftIndentation = 4

        ' Add above space, empty lines.
        file.Sections(2).TrailingComment.EmptyLinesBefore = 2
    End Sub

    Private Sub Save()
        Dim options As New IniOptions()
        Dim iniFile As New IniFile(options)
        iniFile.Sections.Add(
            New IniSection(iniFile, "Section 1",
                New IniKey(iniFile, "Key 1.1", "Value 1.1"),
                New IniKey(iniFile, "Key 1.2", "Value 1.2"),
                New IniKey(iniFile, "Key 1.3", "Value 1.3"),
                New IniKey(iniFile, "Key 1.4", "Value 1.4")))

        ' Save file to path.
        iniFile.Save("..\..\..\MadMilkman.Ini.Samples.Files\Save Example.ini")

        ' Save file to stream.
        Using stream As Stream = File.Create("..\..\..\MadMilkman.Ini.Samples.Files\Save Example.ini")
            iniFile.Save(stream)
        End Using

        ' Save file's content to string.
        Dim contentWriter As New StringWriter()
        iniFile.Save(contentWriter)
        Dim iniContent As String = contentWriter.ToString()

        Console.WriteLine(iniContent)
    End Sub

    Private Sub Custom()
        ' Create new file with custom formatting.
        Dim file As New IniFile(
                        New IniOptions() With {
                            .CommentStarter = IniCommentStarter.Hash,
                            .KeyDelimiter = IniKeyDelimiter.Colon,
                            .KeySpaceAroundDelimiter = True,
                            .SectionWrapper = IniSectionWrapper.CurlyBrackets,
                            .Encoding = Encoding.UTF8
                        })

        ' Load file.
        file.Load("..\..\..\MadMilkman.Ini.Samples.Files\Custom Example Input.ini")

        ' Change first section's fourth key's value.
        file.Sections(0).Keys(3).Value = "NEW VALUE"

        ' Save file.
        file.Save("..\..\..\MadMilkman.Ini.Samples.Files\Custom Example Output.ini")
    End Sub

    Private Sub Copy()
        ' Create new file.
        Dim file As New IniFile()

        ' Add new content.
        Dim section As IniSection = file.Sections.Add("Section")
        Dim key As IniKey = section.Keys.Add("Key")

        ' Add duplicate section.
        file.Sections.Add(section.Copy())

        ' Add duplicate key.
        section.Keys.Add(key.Copy())

        ' Create new file.
        Dim newFile As New IniFile(New IniOptions())

        ' Import first file's section to second file.
        newFile.Sections.Add(section.Copy(newFile))
    End Sub

    Private Sub Parse()
        Dim file As New IniFile()
        Dim content As String = "[Player]" + Environment.NewLine +
                                "Full Name = John Doe" + Environment.NewLine +
                                "Birthday = 12/31/1999" + Environment.NewLine +
                                "Married = Yes" + Environment.NewLine +
                                "Score = 9999999" + Environment.NewLine +
                                "Game Time = 00:59:59"
        Using stream As Stream = New MemoryStream(Encoding.ASCII.GetBytes(content))
            file.Load(stream)
        End Using

        ' Map 'yes' value as 'true' boolean.
        file.ValueMappings.Add("yes", True)
        ' Map 'no' value as 'false' boolean.
        file.ValueMappings.Add("no", False)

        Dim playerSection As IniSection = file.Sections("Player")

        '  Retrieve player's name.
        Dim playerName As String = playerSection.Keys("Full Name").Value

        ' Retrieve player's birthday as DateTime.
        Dim playerBirthday As DateTime
        playerSection.Keys("Birthday").TryParseValue(playerBirthday)

        ' Retrieve player's marital status as bool.
        ' TryParseValue succeeds due to the mapping of 'yes' value to 'true' boolean.
        Dim playerMarried As Boolean
        playerSection.Keys("Married").TryParseValue(playerMarried)

        ' Retrieve player's score as long.
        Dim playerScore As Long
        playerSection.Keys("Score").TryParseValue(playerScore)

        ' Retrieve player's game time as TimeSpan.
        Dim playerGameTime As TimeSpan
        playerSection.Keys("Game Time").TryParseValue(playerGameTime)
    End Sub

    Private Sub BindInternal()
        Dim file As New IniFile()
        Dim content As String = "[Machine Settings]" + Environment.NewLine +
                                "Program Files = C:\Program Files" + Environment.NewLine +
                                "[Application Settings]" + Environment.NewLine +
                                "Name = Example App" + Environment.NewLine +
                                "Version = 1.0" + Environment.NewLine +
                                "Full Name = @{Name} v@{Version}" + Environment.NewLine +
                                "Executable Path = @{Machine Settings|Program Files}\@{Name}.exe"
        Using stream As Stream = New MemoryStream(Encoding.ASCII.GetBytes(content))
            file.Load(stream)
        End Using

        ' Bind placeholders with file's content, internal information.
        file.ValueBinding.Bind()

        ' Retrieve application's full name, value is 'Example App v1.0'.
        Dim appFullName As String = file.Sections("Application Settings").Keys("Full Name").Value

        ' Retrieve application's executable path, value is 'C:\\Program Files\\Example App.exe'.
        Dim appExePath As String = file.Sections("Application Settings").Keys("Executable Path").Value
    End Sub

    Private Sub BindExternal()
        Dim file As New IniFile()
        Dim content As String = "[User's Settings]" + Environment.NewLine +
                                "Nickname = @{User Alias}" + Environment.NewLine +
                                "Full Name = @{User Name} @{User Surname}" + Environment.NewLine +
                                "Profile Page = @{Homepage}/Profiles/@{User Alias}"
        Using stream As Stream = New MemoryStream(Encoding.ASCII.GetBytes(content))
            file.Load(stream)
        End Using

        ' Bind placeholders with user's data, external information.
        file.ValueBinding.Bind(
            New Dictionary(Of String, String)() From
            {
                {"User Alias", "Johny"},
                {"User Name", "John"},
                {"User Surname", "Doe"}
            })

        ' Bind 'Homepage' placeholder with 'www.example.com' value.
        file.ValueBinding.Bind(
            New KeyValuePair(Of String, String)("Homepage", "www.example.com"))

        ' Retrieve user's full name, value is 'John Doe'.
        Dim userFullName As String = file.Sections("User's Settings").Keys("Full Name").Value

        ' Retrieve user's profile page, value is 'www.example.com/Profiles/Johny'.
        Dim userProfilePage As String = file.Sections("User's Settings").Keys("Profile Page").Value
    End Sub

    Sub Main()
        HelloWorld()

        Create()

        Load()

        Style()

        Save()

        Custom()

        Copy()

        Parse()

        BindInternal()

        BindExternal()
    End Sub

End Module