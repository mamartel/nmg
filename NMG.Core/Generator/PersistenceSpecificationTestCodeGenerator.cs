using System;
using System.CodeDom;
using System.Linq;

using NMG.Core.Domain;
using NMG.Core.TextFormatter;

namespace NMG.Core.Generator
{
    public class PersistenceSpecificationTestCodeGenerator : AbstractCodeGenerator
    {
        public PersistenceSpecificationTestCodeGenerator(ApplicationPreferences appPrefs, Table table) :
            base(appPrefs.FolderPath, "Testing", appPrefs.TableName, appPrefs.NameSpaceMap, appPrefs.AssemblyName, appPrefs.Sequence, table, appPrefs)
        {

        }

        public override void Generate(bool writeToFile = true)
        {
            var pascalCaseTextFormatter = new PascalCaseTextFormatter { PrefixRemovalList = applicationPreferences.FieldPrefixRemovalList };

            string className = pascalCaseTextFormatter.FormatSingular(Table.Name);
            string testName = $"{applicationPreferences.ClassNamePrefix}{className}_Test";

            var testMethod = BuildTestMethod(className);

            var compileUnit = new CodeGenerationHelper().GetCodeCompileUnit(nameSpace, "PersistenceTests");
            var type = compileUnit.Namespaces[0].Types[0];
            type.Members.Add(testMethod);

            var generatedCode = GenerateCode(compileUnit, testName);
            if (writeToFile)
            {
                WriteToFile(generatedCode, className);
            }
        }

        protected override string AddStandardHeader(string entireContent)
        {
            string @namespace;
            switch (applicationPreferences.PersistenceTestingFramework)
            {
                case PersistenceTestingFramework.FluentNHibernate:
                    @namespace = "using FluentNHibernate;";
                    break;
                case PersistenceTestingFramework.NHibernatePersistenceTesting:
                    @namespace = "using NHibernate.PersistenceTesting;";
                    break;
                default:
                    throw new Exception("Unknown value for PersistenceTestingFramework");
            }

            return base.AddStandardHeader(Environment.NewLine + @namespace + entireContent);
        }

        protected override string CleanupGeneratedFile(string generatedContent) => String.Empty;
        
        private CodeMemberMethod BuildTestMethod(string className)
        {
            var testMethod = new CodeMemberMethod { Name = $"{className}_Test", Attributes = MemberAttributes.Public };
            testMethod.CustomAttributes.Add(new CodeAttributeDeclaration(applicationPreferences.TestMethodAttributeName));
            testMethod.Statements.Add(new CodeSnippetStatement($"{TABS}new PersistenceSpecification<{className}>(session)."));

            AddPropertyChecks(testMethod);

            testMethod.Statements.Add(new CodeSnippetStatement($"{TABS}{TABS}VerifyTheMappings();"));

            return testMethod;
        }

        private void AddPropertyChecks(CodeMemberMethod testMethod)
        {
            foreach (var column in Table.Columns.Where(x => !x.IsForeignKey || !applicationPreferences.IncludeForeignKeys).OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
            {
                testMethod.Statements.Add(new CodeSnippetStatement($"{TABS}{TABS}CheckProperty(x => x.{column.Name}, {GetTestValue(column)})."));
            }
        }

        private static string GetTestValue(Column column)
        {
            if (DataTypeMapper.IsNumericType(column.MappedDataType))
                return "1";

            if (column.MappedDataType == typeof(string))
                return $"\"{column.Name}\"";

            if (column.MappedDataType.IsEnum)
                return Enum.GetValues(column.MappedDataType).GetValue(0).ToString();

            if (column.MappedDataType == typeof(DateTime))
                return "DateTime.Now";

            if (column.MappedDataType == typeof(bool))
                return "true";

            if (column.MappedDataType == typeof(byte))
                return "0";

            return null;
        }
    }
}