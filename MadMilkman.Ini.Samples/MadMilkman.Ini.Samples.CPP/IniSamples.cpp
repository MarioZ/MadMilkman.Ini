#include "stdafx.h"
using namespace System;
using namespace System::IO;
using namespace System::Text;
using namespace MadMilkman::Ini;


void HelloWorld()
{
	// Create new file.
	IniFile ^file = gcnew IniFile();

	// Add new section.
	IniSection ^section = file->Sections->Add("Section Name");

	// Add new key and its value.
	IniKey ^key = section->Keys->Add("Key Name", "Hello World");

	// Read file's specific value.
	Console::WriteLine(file->Sections["Section Name"]->Keys["Key Name"]->Value);
}

void Create()
{
	// Create new file with default formatting.
	IniFile ^file = gcnew IniFile(gcnew IniOptions());
	
	// Add new content.
	IniSection ^section = gcnew IniSection(file, IniSection::GlobalSectionName);
	IniKey ^key = gcnew IniKey(file, "Key 1", "Value 1");
	file->Sections->Add(section);
	section->Keys->Add(key);

	// Add new content.
	file->Sections->Add("Section 2")->Keys->Add("Key 2", "Value 2");

	// Add new content.
	file->Sections->Add(
		gcnew IniSection(file, "Section 3",
			gcnew IniKey(file, "Key 3", "Value 3")));
}

void Load()
{
	IniOptions ^options = gcnew IniOptions();
	IniFile ^iniFile = gcnew IniFile(options);

	// Load file from path.
	iniFile->Load("..\\MadMilkman.Ini.Samples.Files\\Load Example.ini");

	// Load file from stream.
	Stream ^fileStream = File::OpenRead("..\\MadMilkman.Ini.Samples.Files\\Load Example.ini");
	try { iniFile->Load(fileStream); }
	finally	{ delete fileStream; }

	// Load file's content from string.
	String ^iniContent = "[Section 1]" + Environment::NewLine + 
						 "Key 1.1 = Value 1.1" + Environment::NewLine + 
						 "Key 1.2 = Value 1.2" + Environment::NewLine + 
						 "Key 1.3 = Value 1.3" + Environment::NewLine + 
						 "Key 1.4 = Value 1.4";
	Stream ^contentStream = gcnew MemoryStream(options->Encoding->GetBytes(iniContent));
	try	{ iniFile->Load(contentStream);	}
	finally	{ delete contentStream;	}

	// Read file's content.
	for each (IniSection ^section in iniFile->Sections)
	{
		Console::WriteLine("SECTION: {0}", section->Name);
		for each (IniKey ^key in section->Keys)
		{
			Console::WriteLine("KEY: {0}, VALUE: {1}", key->Name, key->Value);
		}
	}
}

void Style()
{
	IniFile ^file = gcnew IniFile();
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
	IniOptions ^options = gcnew IniOptions();
	IniFile ^iniFile = gcnew IniFile(options);
	iniFile->Sections->Add(
		gcnew IniSection(iniFile, "Section 1",
			gcnew IniKey(iniFile, "Key 1.1", "Value 1.1"),
			gcnew IniKey(iniFile, "Key 1.2", "Value 1.2"),
			gcnew IniKey(iniFile, "Key 1.3", "Value 1.3"),
			gcnew IniKey(iniFile, "Key 1.4", "Value 1.4")));

	// Save file to path.
	iniFile->Save("..\\MadMilkman.Ini.Samples.Files\\Save Example.ini");

	// Save file to stream.
	Stream ^fileStream = File::Create("..\\MadMilkman.Ini.Samples.Files\\Save Example.ini");
	try { iniFile->Save(fileStream); }
	finally	{ delete fileStream; }

	// Save file's content to string.
	String ^iniContent;
	Stream ^contentStream = gcnew MemoryStream();
	try
	{
		iniFile->Save(contentStream);
		iniContent = (gcnew StreamReader(contentStream, options->Encoding))->ReadToEnd();
	}
	finally	{ delete contentStream; }

	Console::WriteLine(iniContent);
}

void Custom()
{
	IniOptions ^options = gcnew IniOptions();
	options->CommentStarter = IniCommentStarter::Hash;
	options->KeyDelimiter = IniKeyDelimiter::Colon;
	options->KeySpaceAroundDelimiter = true;
	options->SectionWrapper = IniSectionWrapper::CurlyBrackets;
	options->Encoding = Encoding::UTF8;
	IniFile ^file = gcnew IniFile(options);

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
	IniFile ^file = gcnew IniFile();

	// Add new content.
	IniSection ^section = file->Sections->Add("Section");
	IniKey ^key = section->Keys->Add("Key");

	// Add duplicate section.
	file->Sections->Add(section->Copy());

	// Add duplicate key.
	section->Keys->Add(key->Copy());

	// Create new file.
	IniFile ^newFile = gcnew IniFile(gcnew IniOptions());

	// Import first file's section to second file.
	newFile->Sections->Add(section->Copy(newFile));
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
}