# ![MadMilkman.Ini component's icon](../blob/master/MadMilkman.Ini/Properties/MadMilkman.Ini.png) MadMilkman.Ini
**MadMilkman.Ini** is a .NET component which simplifies processing of INI files and requires only a minimum **.NET Framework version 2.0**.

## Advantages:
* Enables reading and writing of various INI file formats.
* Enables accessing and updating INI file's content.
* Enables creating and deleting INI file's content.
* Enables copying and merging multiple INI file's contents.

## Installation
You can use this component in any way that suits you:
* As _private assembly_ by adding a DLL inside your project.
* As _shared assembly_ by downloading and installing [MadMilkman.Ini.Setup.msi](../blob/master/MadMilkman.Ini.Setup.msi).
* As _package_ via NuGet:
```powershell
PM> Install-Package MadMilkman.Ini
```

## First steps:
1. Create new .NET project.
2. Add new reference to MadMilkman.Ini.dll.
3. Add MadMilkman.Ini namespace.
  * [C#]  `using MadMilkman.Ini;`
  * [VB]  `Import MadMilkman.Ini`
  * [C++] `using namespace MadMilkman::Ini;`
4. Write some INI files processing code.
  * Use code samples written in C#, VB and C++ as starting point.
  * Use MadMilkman.Ini.Documentation.chm to see all API references.

### [C#] Hello World
```csharp
// Create new file.
IniFile file = new IniFile();

// Add new section.
IniSection section = file.Sections.Add("Section Name");

// Add new key and its value.
IniKey key = section.Keys.Add("Key Name", "Hello World");

// Read file's specific value.
Console.WriteLine(file.Sections["Section Name"].Keys["Key Name"].Value);
```

### [VB] Hello World
```vb.net
' Create new file.
Dim file As New IniFile()

' Add new section.
Dim section As IniSection = file.Sections.Add("Section Name")

' Add new key and its value.
Dim key As IniKey = section.Keys.Add("Key Name", "Hello World")

' Read file's specific value.
Console.WriteLine(file.Sections("Section Name").Keys("Key Name").Value)
```

### [C++] Hello World
```cpp
// Create new file.
IniFile ^file = gcnew IniFile();

// Add new section.
IniSection ^section = file->Sections->Add("Section Name");

// Add new key and its value.
IniKey ^key = section->Keys->Add("Key Name", "Hello World");

// Read file's specific value.
Console::WriteLine(file->Sections["Section Name"]->Keys["Key Name"]->Value);
```
