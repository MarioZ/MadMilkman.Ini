#region License & Copyright Notice
/* Copyright 2015 Mario Zorica.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. */
#endregion

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
    /// <para>To view INI file's structure representation see <see href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm#diagram" target="_self">IniFile's Content Hierarchy Diagram</see>.</para>
    /// </remarks>
    /// <seealso href="c49dc3a5-866f-4d2d-8f89-db303aceb5fe.htm" target="_self">Overview</seealso>
    /// <seealso href="http://en.wikipedia.org/wiki/INI_file">INI file format on Wikipedia.</seealso>
    public sealed class IniFile
    {
        internal readonly IniOptions options;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IniSectionCollection sections;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IniValueMappings valueMappings;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IniValueBinding valueBinding;

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
        /// Gets the mappings of <see cref="IniKey.Value"/>s and their results, used in <see cref="O:MadMilkman.Ini.IniKey.TryParseValue"/> methods.
        /// </summary>
        public IniValueMappings ValueMappings
        {
            get
            {
                if (this.valueMappings == null)
                    this.valueMappings = new IniValueMappings();
                return this.valueMappings;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal bool HasValueMappings { get { return this.valueMappings != null; } }

        /// <summary>
        /// Gets the object that exposes binding operations, which are executed with <see cref="O:MadMilkman.Ini.IniValueBinding.Bind"/> methods.
        /// </summary>
        public IniValueBinding ValueBinding
        {
            get
            {
                if (this.valueBinding == null)
                    this.valueBinding = new IniValueBinding(this);
                return this.valueBinding;
            }
        }

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