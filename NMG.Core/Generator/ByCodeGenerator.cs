using System;
using System.CodeDom;
using System.Linq;
using System.Text;
using NMG.Core.Domain;
using NMG.Core.ByCode;
using NMG.Core.TextFormatter;

namespace NMG.Core.Generator
{
    public class ByCodeGenerator : AbstractCodeGenerator
    {
        private readonly IApplicationSettings appPrefs;

        public ByCodeGenerator(IApplicationSettings appPrefs, Table table) :
            base(appPrefs.FolderPath, "Mapping", table.Name, appPrefs.NameSpaceMap, appPrefs.AssemblyName, appPrefs.Sequence, table, appPrefs)
        {
            this.appPrefs = appPrefs;
            language = this.appPrefs.Language;
        }

        public override void Generate(bool writeToFile = true)
        {
            var entityClassName = ClassFormatter.FormatSingular(Table.Name);
            var mapClassName = $"{entityClassName}Map";

            var compileUnit = GetCompleteCompileUnit(mapClassName, entityClassName);
            var generateCode = GenerateCode(compileUnit, mapClassName);

            if (writeToFile)
            {
                WriteToFile(generateCode, mapClassName);
            }
        }

        protected override string CleanupGeneratedFile(string generatedContent)
        {
            return generatedContent;
        }

        private CodeCompileUnit GetCompleteCompileUnit(string mapName, string className)
        {
            var codeGenerationHelper = new CodeGenerationHelper();
            var compileUnit = codeGenerationHelper.GetCodeCompileUnit(nameSpace, mapName);

            var newType = compileUnit.Namespaces[0].Types[0];

            newType.IsPartial = appPrefs.GeneratePartialClasses;

            switch (appPrefs.Language)
            {
                case Language.CSharp:
                    newType.BaseTypes.Add($"ClassMapping<{className}>");
                    break;
                case Language.VB:
                    newType.BaseTypes.Add($"ClassMapping(Of {className})");
                    break;
            }

            var constructor = new CodeConstructor { Attributes = MemberAttributes.Public };

            // Table Name - Only ouput if table is different than the class name.
            if (!string.Equals(Table.Name, className, StringComparison.InvariantCultureIgnoreCase))
            {
                constructor.Statements.Add(new CodeSnippetStatement(TABS + "Table(\"" + Table.Name + "\");"));
            }
            // Scheme / Owner Name
            if (!string.IsNullOrEmpty(Table.Owner))
            {
                constructor.Statements.Add(new CodeSnippetStatement(TABS + "Schema(\"" + Table.Owner + "\");"));
            }

            constructor.Statements.Add(new CodeSnippetStatement(TABS + string.Format("Lazy({0});", appPrefs.UseLazy ? "true" : "false")));

            var mapper = new DBColumnMapper(appPrefs);

            // Id or ComposedId Map 
            if (Table.PrimaryKey != null)
            {
                if (UsesSequence)
                {
                    constructor.Statements.Add(
                        new CodeSnippetStatement(TABS +
                                                 mapper.IdSequenceMap(Table.PrimaryKey.Columns[0], appPrefs.Sequence,
                                                                      FieldFormatter)));
                }
                else if (Table.PrimaryKey.Type == PrimaryKeyType.PrimaryKey)
                {
                    constructor.Statements.Add(
                        new CodeSnippetStatement(TABS + mapper.IdMap(Table.PrimaryKey.Columns[0], FieldFormatter)));
                }
                else if (Table.PrimaryKey.Type == PrimaryKeyType.CompositeKey)
                {
                    var pkColumns = Table.PrimaryKey.Columns;
                    constructor.Statements.Add(
                        new CodeSnippetStatement(TABS + mapper.CompositeIdMap(pkColumns, FieldFormatter)));
                }
            }

            // Property Map
            foreach (var column in Table.Columns.Where(x => !x.IsPrimaryKey && (!x.IsForeignKey || !appPrefs.IncludeForeignKeys)).OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                constructor.Statements.Add(new CodeSnippetStatement(TABS + mapper.Map(column, FieldFormatter, appPrefs.IncludeLengthAndScale)));
            }

            // Many To One Mapping
            foreach (var fk in Table.ForeignKeys.Where(fk => fk.Columns.First().IsForeignKey && appPrefs.IncludeForeignKeys))
            {
                constructor.Statements.Add(new CodeSnippetStatement(mapper.Reference(fk, FieldFormatter)));
            }

            // Bag 
            if (appPrefs.IncludeHasMany)
            {
                Table.HasManyRelationships.ToList().ForEach(x => constructor.Statements.Add(new CodeSnippetStatement(mapper.Bag(x, FieldFormatter))));
            }

            newType.Members.Add(constructor);

            // Strip out any semicolons 
            if (appPrefs.Language == Language.VB)
            {
                foreach (CodeSnippetStatement statement in constructor.Statements)
                {
                    statement.Value = statement.Value.Replace(";", string.Empty);
                }
            }

            return compileUnit;
        }

        protected override string AddStandardHeader(string entireContent)
        {
            var builder = new StringBuilder();
            if (appPrefs.Language == Language.CSharp)
            {
                builder.AppendLine("using System;");
                builder.AppendLine("using System.Text;");
                builder.AppendLine("using System.Collections.Generic;");
                builder.AppendLine("using System.Linq;");
                builder.AppendLine("using NHibernate.Mapping.ByCode.Conformist;");
                builder.AppendLine("using NHibernate.Mapping.ByCode;");
                builder.AppendFormat("using {0};", appPrefs.NameSpace);
                builder.AppendLine();
                if (appPrefs.ForeignEntityCollectionType.Contains("Iesi.Collections"))
                    builder.AppendLine("using Iesi.Collections.Generic;");
            }
            else if (appPrefs.Language == Language.VB)
            {
                builder.AppendLine("Imports System");
                builder.AppendLine("Imports System.Text");
                builder.AppendLine("Imports System.Collections.Generic");
                builder.AppendLine("Imports System.Linq");
                builder.AppendLine("Imports NHibernate.Mapping.ByCode.Conformist");
                builder.AppendLine("Imports NHibernate.Mapping.ByCode");
                builder.AppendFormat("Imports {0}", appPrefs.NameSpace);
                builder.AppendLine();
                if (appPrefs.ForeignEntityCollectionType.Contains("Iesi.Collections"))
                    builder.AppendLine("Imports Iesi.Collections.Generic");

                entireContent = entireContent.Replace("Option Strict Off", string.Empty);
                entireContent = entireContent.Replace("Option Explicit On", string.Empty);
            }

            builder.Append(entireContent);
            return builder.ToString();
        }
    }
}

