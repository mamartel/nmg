using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using NMG.Core.Domain;
using NMG.Core.TextFormatter;

namespace NMG.Core.Generator
{
    public abstract class AbstractGenerator : IGenerator
    {
        protected Table Table;
        protected string assemblyName;
        protected string filePath;
        protected string nameSpace;
        protected string sequenceName;
        protected string tableName;
		internal const string TABS = "\t\t\t";
    	protected string ClassNamePrefix { get; set;}
        protected IApplicationSettings applicationPreferences;

        protected AbstractGenerator(string filePath, string specificFolder, string tableName, string nameSpace, string assemblyName, string sequenceName, Table table, IApplicationSettings appPrefs)
        {
            this.filePath = filePath;
            if(appPrefs.GenerateInFolders)
            {
                this.filePath = Path.Combine(filePath, specificFolder);
                if(!this.filePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    this.filePath = this.filePath + Path.DirectorySeparatorChar;
                }
            }
            this.tableName = tableName;
            this.nameSpace = nameSpace;
            this.assemblyName = assemblyName;
            this.sequenceName = sequenceName;
            Table = table;
            ClassFormatter = TextFormatterFactory.GetClassTextFormatter(appPrefs);
            FieldFormatter = TextFormatterFactory.GetFieldTextFormatter(appPrefs);
            this.applicationPreferences = appPrefs;
        }

        public bool UsesSequence
        {
            get
            {
                return !String.IsNullOrEmpty(sequenceName);
            }
        }

        public ITextFormatter ClassFormatter { get; set; }

        public ITextFormatter FieldFormatter { get; set; }

        public string GeneratedCode { get; set; }

        public abstract void Generate(bool writeToFile = true);

        protected string WriteToString(CodeCompileUnit compileUnit, CodeDomProvider provider)
        {
            var streamWriter = new StringWriter();
            using (provider)
            {
                var textWriter = new IndentedTextWriter(streamWriter, "    ");
                using (textWriter)
                {
                    using (streamWriter)
                    {
                        if (applicationPreferences.QuietNullableWarnings)
                            textWriter.WriteLine("#nullable disable warnings");

                        var options = new CodeGeneratorOptions { BlankLinesBetweenMembers = false };
                        provider.GenerateCodeFromCompileUnit(compileUnit, textWriter, options);

                        if (applicationPreferences.QuietNullableWarnings)
                            textWriter.WriteLine("#nullable restore warnings");
                    }
                }
            }

            return CleanupGeneratedFile(streamWriter.ToString());
        }

        protected abstract string CleanupGeneratedFile(string generatedContent);
    }
}