Imports System
Imports System.IO
Imports System.Text
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
        file.Sections.Add( _
            New IniSection(file, "Section 3", _
                New IniKey(file, "Key 3", "Value 3")))
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
        Dim iniContent As String = "[Section 1]" + Environment.NewLine + _
                                   "Key 1.1 = Value 1.1" + Environment.NewLine + _
                                   "Key 1.2 = Value 1.2" + Environment.NewLine + _
                                   "Key 1.3 = Value 1.3" + Environment.NewLine + _
                                   "Key 1.4 = Value 1.4"
        Using stream As Stream = New MemoryStream(options.Encoding.GetBytes(iniContent))
            iniFile.Load(stream)
        End Using

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
            New IniSection(iniFile, "Section 1", _
                New IniKey(iniFile, "Key 1.1", "Value 1.1"), _
                New IniKey(iniFile, "Key 1.2", "Value 1.2"), _
                New IniKey(iniFile, "Key 1.3", "Value 1.3"), _
                New IniKey(iniFile, "Key 1.4", "Value 1.4")))

        ' Save file to path.
        iniFile.Save("..\..\..\MadMilkman.Ini.Samples.Files\Save Example.ini")

        ' Save file to stream.
        Using stream As Stream = File.Create("..\..\..\MadMilkman.Ini.Samples.Files\Save Example.ini")
            iniFile.Save(stream)
        End Using

        ' Save file's content to string.
        Dim iniContent As String
        Using stream As Stream = New MemoryStream()
            iniFile.Save(stream)
            iniContent = New StreamReader(stream, options.Encoding).ReadToEnd()
        End Using

        Console.WriteLine(iniContent)
    End Sub

    Private Sub Custom()
        ' Create new file with custom formatting.
        Dim file As New IniFile( _
                        New IniOptions() With { _
                            .CommentStarter = IniCommentStarter.Hash, _
                            .KeyDelimiter = IniKeyDelimiter.Colon, _
                            .KeySpaceAroundDelimiter = True, _
                            .SectionWrapper = IniSectionWrapper.CurlyBrackets, _
                            .Encoding = Encoding.UTF8 _
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

    Sub Main()
        HelloWorld()

        Create()

        Load()

        Style()

        Save()

        Custom()

        Copy()
    End Sub

End Module