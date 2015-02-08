using System;
using System.IO;

namespace MadMilkman.Ini.Tests
{
    public static class IniUtilities
    {
        public static IniFile LoadIniFileContent(string iniFileContent, IniOptions options)
        {
            IniFile file = new IniFile(options);

            using (var stream = new MemoryStream(options.Encoding.GetBytes(iniFileContent)))
                file.Load(stream);

            return file;
        }

        public static string[] SaveIniFileContent(IniFile file, IniOptions options)
        {
            string iniFileContent;
            using (var stream = new MemoryStream())
            {
                file.Save(stream);
                iniFileContent = new StreamReader(stream, options.Encoding).ReadToEnd();
            }

            return iniFileContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}
