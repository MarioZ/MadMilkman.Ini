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

void Encrypt()
{
	// Enable file's protection by providing an encryption password.
	IniOptions^ options = gcnew IniOptions();
	options->EncryptionPassword = "M4dM1lkM4n.1n1";
	IniFile^ file = gcnew IniFile(options);

	IniSection^ section = file->Sections->Add("User's Account");
	section->Keys->Add("Username", "John Doe");
	section->Keys->Add("Password", "P@55\\/\\/0|2D");

	// Save and encrypt the file.
	file->Save("..\\MadMilkman.Ini.Samples.Files\\Encrypt Example.ini");

	file->Sections->Clear();

	// Load and dencrypt the file.
	file->Load("..\\MadMilkman.Ini.Samples.Files\\Encrypt Example.ini");

	Console::WriteLine("User Name: {0}", file->Sections[0]->Keys["Username"]->Value);
	Console::WriteLine("Password: {0}", file->Sections[0]->Keys["Password"]->Value);
}

void Compress()
{
	// Enable file's size reduction.
	IniOptions^ options = gcnew IniOptions();
	options->Compression = true;
	IniFile^ file = gcnew IniFile(options);

	for (int i = 1; i <= 100; i++)
	{
	    file->Sections->Add("Section " + i)->Keys->Add("Key " + i, "Value " + i);
	}

	// Save and compress the file.
	file->Save("..\\MadMilkman.Ini.Samples.Files\\Compress Example.ini");

	file->Sections->Clear();

	// Load and decompress the file.
	file->Load("..\\MadMilkman.Ini.Samples.Files\\Compress Example.ini");

	Console::WriteLine(file->Sections->Count);
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

private ref class BindingCustomizationSample {
public:
	void BindCustomize()
	{
		IniFile^ file = gcnew IniFile();
		String^ content = "[Player]" + Environment::NewLine +
						  "Name = @{Name}" + Environment::NewLine +
						  "Surname = @{Surname}" + Environment::NewLine +
						  "Adult = @{Age}" + Environment::NewLine +
						  "Medal = @{Rank}";
		file->Load(gcnew StringReader(content));

		// Customize binding operation.
		file->ValueBinding->Binding += gcnew EventHandler<IniValueBindingEventArgs^>(this, &BindingCustomizationSample::CustomEventHandler);

		// Execute binding operation.
		Dictionary<String^, String^>^ dataSource = gcnew Dictionary<String^, String^>();
		dataSource->Add("Name", "John");
		dataSource->Add("Age", "20");
		dataSource->Add("Rank", "1");
		file->ValueBinding->Bind(dataSource);
	}

	void CustomEventHandler(Object^ sender, IniValueBindingEventArgs^ e)
	{
		if (!e->IsValueFound)
		{
			e->Value = "UNKNOWN";
			return;
		}
		if (e->PlaceholderKey->Name->Equals("Adult") && e->PlaceholderName->Equals("Age"))
		{
			int age;
			if (int::TryParse(e->Value, age))
			{
				e->Value = (age >= 18) ? "YES" : "NO";
			}
			else
			{
				e->Value = "UNKNOWN";
			}
			return;
		}
		if (e->PlaceholderKey->Name->Equals("Medal") && e->PlaceholderName->Equals("Rank"))
		{
			int rank;
			if (int::TryParse(e->Value, rank))
			{
				switch (rank)
				{
					case 1:
						e->Value = "GOLD";
						break;
					case 2:
						e->Value = "SILVER";
						break;
					case 3:
						e->Value = "BRONCE";
						break;
					default:
						e->Value = "NONE";
						break;
				}
			}
			else
			{
				e->Value = "UNKNOWN";
			}
		return;
		}
	}
};

// Custom type used for serialization sample.
private ref class GameCharacter
{
public:
	property String^ Name;

	// Serialize this property as a key with "Sword" name.
	[IniSerialization("Sword")]
	property double Attack;

	// Serialize this property as a key with "Shield" name.
	[IniSerialization("Shield")]
	property double Defence;

	// Ignore serializing this property.
	[IniSerialization(true)]
	property double Health;

	GameCharacter()
	{
		this->Health = 100;
	}
};

void Serialize()
{
	IniFile^ file = gcnew IniFile();
	IniSection^ section = file->Sections->Add("User's Character");

	GameCharacter^ character = gcnew GameCharacter();
	character->Name = "John";
	character->Attack = 5.5;
	character->Defence = 1;
	character->Health = 75;

	// Serialize GameCharacter object into section's keys.
	section->Serialize(character);

	// Deserialize section into GameCharacter object.
	GameCharacter^ savedCharacter = section->Deserialize<GameCharacter^>();

	Console::WriteLine(section->Keys["Name"]->Value);
	Console::WriteLine(savedCharacter->Name);
	Console::WriteLine(section->Keys["Sword"]->Value);
	Console::WriteLine(savedCharacter->Attack);
	Console::WriteLine(section->Keys["Shield"]->Value);
	Console::WriteLine(savedCharacter->Defence);
}

void main()
{
	HelloWorld();

	Create();

	Load();

	Style();

	Save();

	Encrypt();

	Compress();

	Custom();

	Copy();

	Parse();

	BindInternal();

	BindExternal();

	BindingCustomizationSample^ sample = gcnew BindingCustomizationSample();
	sample->BindCustomize();

	Serialize();
}