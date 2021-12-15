# ![MadMilkman.Ini component's icon](https://raw.githubusercontent.com/MarioZ/MadMilkman.Ini/64ac0180cc44afd72f8e78945d62ce10ed585f3d/MadMilkman.Ini/Properties/MadMilkman.Ini.png) MadMilkman.Ini
**MadMilkman.Ini** is a .NET component which simplifies processing of INI files and requires only a minimum **.NET Framework version 2.0**.
It is 100% managed code (C#), **compatible with Mono framework**, which provides an easy to use programming interface.

## Advantages:
* Enables reading and writing of various INI file formats.
* Enables easy manipulation of INI file's content.
* Enables copying and merging multiple INI file's contents.
* Enables encrypting and decrypting INI files.
* Enables compressing and decompressing INI files.
* Enables serializing and deserializing custom types into an INI content.

## Installation:
You can use this component in any way that suits you:
* As _private assembly_ by adding [MadMilkman.Ini.dll](https://github.com/MarioZ/MadMilkman.Ini/raw/2fddab97474a1c801bd1be6ddbbf33f12261d272/MadMilkman.Ini.zip) inside your project.
* As _shared assembly_ by installing [MadMilkman.Ini.Setup.msi](https://github.com/MarioZ/MadMilkman.Ini/raw/2fddab97474a1c801bd1be6ddbbf33f12261d272/MadMilkman.Ini.Setup.msi).
* As _package_ via [NuGet](https://www.nuget.org/packages/MadMilkman.Ini/1.0.6):
```powershell
PM> Install-Package MadMilkman.Ini -Version 1.0.6
```

## First steps:
1. Create new .NET project.
2. Add new reference to MadMilkman.Ini.dll.
3. Add MadMilkman.Ini namespace.
  * C# - `using MadMilkman.Ini;`
  * VB.NET - `Import MadMilkman.Ini`
  * C++/CLI - `using namespace MadMilkman::Ini;`
4. Write some INI files processing code.
  * Use code samples located in MadMilkman.Ini.Samples, written in C#, VB.NET and C++/CLI as starting point.
  * Read [MadMilkman.Ini.Documentation.chm](https://github.com/MarioZ/MadMilkman.Ini/raw/2fddab97474a1c801bd1be6ddbbf33f12261d272/MadMilkman.Ini.Documentation.zip) to learn more about the component and its API references.

## Feedback & Support:
Please feel free to contact me with any questions, suggestions or issues regarding the MadMilkman.Ini component, I will be more than happy to provide a help.
Also if you found the component useful or useless I would be interested in hearing about it.

## Overview
MadMilkman.Ini provides a simple and intuitive programming interface which makes it very easy to create new or process existing INI files. Because INI file format is loosely defined and has no real standard, different files can have different format. By default MadMilkman.Ini processes the following format (however it is possible to define a custom formatting via IniOptions class):

```cfg
;Section trailing comment
[Section name]
Key name = Key value ;Key leading comment
```

### C# #
```csharp
// Create new file with a default formatting.
IniFile file = new IniFile();

// Add new section.
IniSection section = file.Sections.Add("Section name");
// Add trailing comment.
section.TrailingComment.Text = "Section trailing comment";

// Add new key and its value.
IniKey key = section.Keys.Add("Key name", "Key value ");
// Add leading comment.
key.LeadingComment.Text = "Key leading comment";
            
// Save file.
file.Save("Sample.ini");
```

### VB.NET
```vb.net
' Create new file with a default formatting.
Dim file As New IniFile()

' Add new section.
Dim section As IniSection = file.Sections.Add("Section name")
' Add trailing comment.
section.TrailingComment.Text = "Section trailing comment"

' Add new key and its value.
Dim key As IniKey = section.Keys.Add("Key name", "Key value ")
' Add leading comment.
key.LeadingComment.Text = "Key leading comment"

' Save file.
file.Save("Sample.ini")
```

### C++/CLI
```cpp
// Create new file with a default formatting.
IniFile^ file = gcnew IniFile();

// Add new section.
IniSection^ section = file->Sections->Add("Section name");
// Add trailing comment.
section->TrailingComment->Text = "Section trailing comment";

// Add new key and its value.
IniKey^ key = section->Keys->Add("Key name", "Key value ");
// Add leading comment.
key->LeadingComment->Text = "Key leading comment";

// Save file.
file->Save("Sample.ini");
```

**Compression** feature enables you to reduce INI file's size.

**Encryption** feature enables you to protect INI file by providing an encryption password.

**Parsing** feature enables you to retrieve the key's value as an instance of a specific type via key's TryParseValue method, all currently supported types are listed in key's IsSupportedValueType method remarks.

**Binding** feature enables you to define placeholders inside a key's value which can be binded (replaced) with an internal or external data source via Bind() method.

**Serialization** feature enables you to serialize an object into section's keys.

More details can be found in [MadMilkman.Ini.Documentation.chm](https://github.com/MarioZ/MadMilkman.Ini/raw/2fddab97474a1c801bd1be6ddbbf33f12261d272/MadMilkman.Ini.Documentation.zip).
