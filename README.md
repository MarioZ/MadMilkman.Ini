# ![MadMilkman.Ini component's icon](../master/MadMilkman.Ini/Properties/MadMilkman.Ini.png) MadMilkman.Ini
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
* As _private assembly_ by adding [MadMilkman.Ini.dll](https://github.com/MarioZ/MadMilkman.Ini/raw/master/MadMilkman.Ini.zip) inside your project.
* As _shared assembly_ by installing [MadMilkman.Ini.Setup.msi](https://github.com/MarioZ/MadMilkman.Ini/raw/master/MadMilkman.Ini.Setup.msi).
* As _package_ via [NuGet](http://www.nuget.org/packages/MadMilkman.Ini):
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
  * Read [MadMilkman.Ini.Documentation.chm](https://github.com/MarioZ/MadMilkman.Ini/raw/master/MadMilkman.Ini.Documentation.chm) to learn more about the component and its API references.
> #### **NOTE:**
> If you are experiencing difficulties with viewing the CHM file's content after downloading it, right-click on CHM file, select "Properties" and under the "General" tab click "Unblock".

## Feedback:
Please feel free to contact me with any questions, suggestions or issues regarding the MadMilkman.Ini component, I will be more than happy to provide a help.
Also if you found the component useful or useless I would be interested in hearing about it.

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
IniFile^ file = gcnew IniFile();

// Add new section.
IniSection^ section = file->Sections->Add("Section Name");

// Add new key and its value.
IniKey^ key = section->Keys->Add("Key Name", "Hello World");

// Read file's specific value.
Console::WriteLine(file->Sections["Section Name"]->Keys["Key Name"]->Value);
```
