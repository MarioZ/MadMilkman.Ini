using System;
using System.IO;
using System.Diagnostics;

namespace MadMilkman.Ini
{
    /// <summary>
    /// In-memory representation of an INI file.
    /// </summary>
    /// <remarks>
    /// <para><see cref="IniFile"/> is a central class of MadMilkman.Ini component.</para>
    /// <para>To define an INI file's format use <see cref="IniOptions"/> object.</para>
    /// <para>To load (read) an INI file from a file's path or a stream use <see cref="O:MadMilkman.Ini.IniFile.Load">IniFile.Load</see> methods.</para>
    /// <para>To save (write) an INI file to a file's path or a stream use <see cref="O:MadMilkman.Ini.IniFile.Save">IniFile.Save</see> methods.</para>
    /// </remarks>
    public sealed class IniFile
    {
        internal readonly IniOptions options;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniSectionCollection sections;

        /// <summary>
        /// Initializes a new instance of <see cref="IniFile"/> class.
        /// </summary>
        public IniFile() : this(new IniOptions()) { }

        /// <summary>
        /// Initializes a new instance of <see cref="IniFile"/> class.
        /// </summary>
        /// <param name="options"><see cref="IniOptions"/> object that defines INI file's format, settings for both <see cref="O:MadMilkman.Ini.IniFile.Load">Load</see> and <see cref="O:MadMilkman.Ini.IniFile.Save">Save</see> methods.</param>
        public IniFile(IniOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            this.options = new IniOptions(options);
            this.sections = new IniSectionCollection(this, options.SectionDuplicate, options.SectionNameCaseSensitive);
        }

        /// <summary>
        /// Gets file's sections.
        /// </summary>
        public IniSectionCollection Sections { get { return this.sections; } }

        /// <summary>
        /// Loads a file from a path.
        /// </summary>
        /// <param name="filePath">Path from which to load a file.</param>
        public void Load(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            using (Stream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                this.Load(fileStream);
        }

        /// <summary>
        /// Loads a file from a stream.
        /// </summary>
        /// <param name="fileStream">Stream from which to load a file.</param>
        public void Load(Stream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            this.Load(new StreamReader(fileStream, this.options.Encoding));

            if (fileStream.CanSeek)
                fileStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Loads a file from a reader.
        /// </summary>
        /// <param name="fileReader">Reader from which to load a file.</param>
        public void Load(TextReader fileReader)
        {
            if (fileReader == null)
                throw new ArgumentNullException("fileReader");

            new IniReader(this.options).Read(this, fileReader);
        }

        /// <summary>
        /// Saves a file to a path.
        /// </summary>
        /// <param name="filePath">Path to which to save a file.</param>
        public void Save(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            using (Stream fileStream = File.Create(filePath))
                this.Save(fileStream);
        }

        /// <summary>
        /// Saves a file to a stream.
        /// </summary>
        /// <param name="fileStream">Stream to which to save a file.</param>
        public void Save(Stream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException("fileStream");

            this.Save(new StreamWriter(fileStream, this.options.Encoding));

            if (fileStream.CanSeek)
                fileStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Saves a file to a writer.
        /// </summary>
        /// <param name="fileWriter">Writer to which to save a file.</param>
        public void Save(TextWriter fileWriter)
        {
            if (fileWriter == null)
                throw new ArgumentNullException("fileWriter");

            new IniWriter(this.options).Write(this, fileWriter);
        }
    }
}
