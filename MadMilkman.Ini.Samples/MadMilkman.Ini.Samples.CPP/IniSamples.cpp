#include "stdafx.h"
using namespace System;
using namespace System::IO;
using namespace System::Text;
using namespace System::Collections::Generic;
using namespace MadMilkman::Ini;


void HelloWorld()
{
	// Create new file.
	IniFile^ file = gcnew IniFile();

	// Add new section.
	IniSection^ section = file->Sections->Add("Section Name");

	// Add new key and its value.
	IniKey^ key = section->Keys->Add("Key Name", "Hello World");

	// Read file's specific value.
	Console::WriteLine(file->Sections["Section Name"]->Keys["Key Name"]->Value);
}

void Create()
{
	// Create new file with default formatting.
	IniFile^ file = gcnew IniFile(gcnew IniOptions());
	
	// Add new content.
	IniSection^ section = gcnew IniSection(file, IniSection::GlobalSectionName);
	IniKey ^key = gcnew IniKey(file, "Key 1", "Value 1");
	file->Sections->Add(section);
	section->Keys->Add(key);

	// Add new content.
	file->Sections->Add("Section 2")->Keys->Add("Key 2", "Value 2");

	// Add new content.
	file->Sections->Add(
		gcnew IniSection(file, "Section 3",
			gcnew IniKey(file, "Key 3.1", "Value 3.1"),
			gcnew IniKey(file, "Key 3.2", "Value 3.2")));

	// Add new content.
	Dictionary<String^, String^>^ collection = gcnew Dictionary<String^, String^>();
	collection->Add("Key 4.1", "Value 4.1");
	collection->Add("Key 4.2", "Value 4.2");
	file->Sections->Add(
		gcnew IniSection(file, "Section 4", collection));
}

void Load()
{
	IniOptions^ options = gcnew IniOptions();
	IniFile^ iniFile = gcnew IniFile(options);

	// Load file from path.
	iniFile->Load("..\\MadMilkman.Ini.Samples.Files\\Load Example.ini");

	// Load file from stream.
	Stream^ fileStream = File::OpenRead("..\\MadMilkman.Ini.Samples.Files\\Load Example.ini");
	try { iniFile->Load(fileStream); }
	finally	{ delete fileStream; }

	// Load file's content from string.
	String^ iniContent = "[Section 1]" + Environment::NewLine + 
						 "Key 1.1 = Value 1.1" + Environment::NewLine + 
						 "Key 1.2 = Value 1.2" + Environment::NewLine + 
						 "Key 1.3 = Value 1.3" + Environment::NewLine + 
						 "Key 1.4 = Value 1.4";
	iniFile->Load(gcnew StringReader(iniContent));

	// Read file's content.
	for each (IniSection^ section in iniFile->Sections)
	{
		Console::WriteLine("SECTION: {0}", section->Name);
		for each (IniKey^ key in section->Keys)
		{
			Console::WriteLine("KEY: {0}, VALUE: {1}", key->Name, key->Value);
		}
	}
}

void Style()
{
	IniFile^ file = gcnew IniFile();
	file->Sections->Add("Section 1")->Keys->Add("Key 1", "Value 1");
	file->Sections->Add("Section 2")->Keys->Add("Key 2", "Value 2");
	file->Sections->Add("Section 3")->Keys->Add("Key 3", "Value 3");

	// Add leading comments.
	file->Sections[0]->LeadingComment->Text = "Section 1 leading comment.";
	file->Sections[0]->Keys[0]->LeadingComment->Text = "Key 1 leading comment.";

	// Add trailing comments.
	file->Sections[1]->TrailingComment->Text = "Section 2 trailing comment->";
	file->Sections[1]->Keys[0]->TrailingComment->Text = "Key 2 trailing comment->";

	// Add left space, indentation.
	file->Sections[1]->LeftIndentation = 4;
	file->Sections[1]->TrailingComment->LeftIndentation = 4;
	file->Sections[1]->Keys[0]->LeftIndentation = 4;
	file->Sections[1]->Keys[0]->TrailingComment->LeftIndentation = 4;

	// Add above space, empty lines.
	file->Sections[2]->TrailingComment->EmptyLinesBefore = 2;
}

void Save()
{
	IniOptions^ options = gcnew IniOptions();
	IniFile^ iniFile = gcnew IniFile(options);
	iniFile->Sections->Add(
		gcnew IniSection(iniFile, "Section 1",
			gcnew IniKey(iniFile, "Key 1.1", "Value 1.1"),
			gcnew IniKey(iniFile, "Key 1.2", "Value 1.2"),
			gcnew IniKey(iniFile, "Key 1.3", "Value 1.3"),
			gcnew IniKey(iniFile, "Key 1.4", "Value 1.4")));

	// Save file to path.
	iniFile->Save("..\\MadMilkman.Ini.Samples.Files\\Save Example.ini");

	// Save file to stream.
	Stream^ fileStream = File::Create("..\\MadMilkman.Ini.Samples.Files\\Save Example.ini");
	try { iniFile->Save(fileStream); }
	finally	{ delete fileStream; }

	// Save file's content to string.
	StringWriter^ contentWriter = gcnew StringWriter();
	iniFile->Save(contentWriter);
	String^ iniContent = contentWriter->ToString();

	Console::WriteLine(iniContent);
}

void Custom()
{
	IniOptions^ options = gcnew IniOptions();
	options->CommentStarter = IniCommentStarter::Hash;
	options->KeyDelimiter = IniKeyDelimiter::Colon;
	options->KeySpaceAroundDelimiter = true;
	options->SectionWrapper = IniSectionWrapper::CurlyBrackets;
	options->Encoding = Encoding::UTF8;
	IniFile^ file = gcnew IniFile(options);

	// Load file.
	file->Load("..\\MadMilkman.Ini.Samples.Files\\Custom Example Input.ini");

	// Change first section's fourth key's value.
	file->Sections[0]->Keys[3]->Value = "NEW VALUE";

	// Save file.
	file->Save("..\\MadMilkman.Ini.Samples.Files\\Custom Example Output.ini");
}

void Copy()
{
	// Create new file.
	IniFile^ file = gcnew IniFile();

	// Add new content.
	IniSection^ section = file->Sections->Add("Section");
	IniKey^ key = section->Keys->Add("Key");

	// Add duplicate section.
	file->Sections->Add(section->Copy());

	// Add duplicate key.
	section->Keys->Add(key->Copy());

	// Create new file.
	IniFile^ newFile = gcnew IniFile(gcnew IniOptions());

	// Import first file's section to second file.
	newFile->Sections->Add(section->Copy(newFile));
}

void Parse()
{
	IniFile^ file = gcnew IniFile();
	String^ content = "[Player]" + Environment::NewLine +
					  "Full Name = John Doe" + Environment::NewLine +
					  "Birthday = 12/31/1999" + Environment::NewLine +
					  "Married = Yes" + Environment::NewLine +
					  "Score = 9999999" + Environment::NewLine +
					  "Game Time = 00:59:59";
	file->Load(gcnew StringReader(content));

	// Map 'yes' value as 'true' boolean.
	file->ValueMappings->Add("yes", true);
	// Map 'no' value as 'false' boolean.
	file->ValueMappings->Add("no", false);

	IniSection^ playerSection = file->Sections["Player"];

	// Retrieve player's name.
	String^ playerName = playerSection->Keys["Full Name"]->Value;

	// Retrieve player's birthday as DateTime.
	DateTime playerBirthday;
	playerSection->Keys["Birthday"]->TryParseValue(playerBirthday);

	// Retrieve player's marital status as bool.
	// TryParseValue succeeds due to the mapping of 'yes' value to 'true' boolean.
	bool playerMarried;
	playerSection->Keys["Married"]->TryParseValue(playerMarried);

	// Retrieve player's score as long.
	long playerScore;
	playerSection->Keys["Score"]->TryParseValue(playerScore);

	// Retrieve player's game time as TimeSpan.
	TimeSpan playerGameTime;
	playerSection->Keys["Game Time"]->TryParseValue(playerGameTime);
}

void BindInternal()
{
	IniFile^ file = gcnew IniFile();
	String^ content = "[Machine Settings]" + Environment::NewLine +
					  "Program Files = C:\\Program Files" + Environment::NewLine +
					  "[Application Settings]" + Environment::NewLine +
					  "Name = Example App" + Environment::NewLine +
					  "Version = 1.0" + Environment::NewLine +
					  "Full Name = @{Name} v@{Version}" + Environment::NewLine +
					  "Executable Path = @{Machine Settings|Program Files}\\@{Name}.exe";
	file->Load(gcnew StringReader(content));

	// Bind placeholders with file's content, internal information.
	file->ValueBinding->Bind();

	// Retrieve application's full name, value is 'Example App v1.0'.
	String^ appFullName = file->Sections["Application Settings"]->Keys["Full Name"]->Value;

	// Retrieve application's executable path, value is 'C:\\Program Files\\Example App.exe'.
	String^ appExePath = file->Sections["Application Settings"]->Keys["Executable Path"]->Value;
}

void BindExternal()
{
	IniFile^ file = gcnew IniFile();
	String^ content = "[User's Settings]" + Environment::NewLine +
					  "Nickname = @{User Alias}" + Environment::NewLine +
					  "Full Name = @{User Name} @{User Surname}" + Environment::NewLine +
					  "Profile Page = @{Homepage}/Profiles/@{User Alias}";
	file->Load(gcnew StringReader(content));

	// Bind placeholders with user's data, external information.
	Dictionary<String^, String^>^ userData = gcnew Dictionary<String^, String^>();
	userData->Add("User Alias", "Johny");
	userData->Add("User Name", "John");
	userData->Add("User Surname", "Doe");
	file->ValueBinding->Bind(userData);

	// Bind 'Homepage' placeholder with 'www.example.com' value.
	file->ValueBinding->Bind(
		KeyValuePair<String^, String^>("Homepage", "www.example.com"));

	// Retrieve user's full name, value is 'John Doe'.
	String^ userFullName = file->Sections["User's Settings"]->Keys["Full Name"]->Value;

	// Retrieve user's profile page, value is 'www.example.com/Profiles/Johny'.
	String^ userProfilePage = file->Sections["User's Settings"]->Keys["Profile Page"]->Value;
}

void main()
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