using System;
using System.Collections.Generic;

using NMG.Core;
using NMG.Core.Domain;

namespace NMG.Tests.Generator
{
    public class TestApplicationSettings : IApplicationSettings
    {
        public string NameSpace { get; set; }
        public string NameSpaceMap { get; set; }
        public string AssemblyName { get; set; }
        public Language Language { get; set; }
        public bool IsFluent { get; set; }
        public bool IsEntityFramework { get; set; }
        public string Sequence { get; set; }
        public FieldGenerationConvention FieldGenerationConvention { get; set; }
        public string FolderPath { get; set; }
        public string DomainFolderPath { get; set; }
        public string ForeignEntityCollectionType { get; set; }
        public string InheritenceAndInterfaces { get; set; }
        public string ClassNamePrefix { get; set; }
        public bool EnableInflections { get; set; }
        public bool GeneratePartialClasses { get; set; }
        public FieldNamingConvention FieldNamingConvention { get; set; }
        public string Prefix { get; set; }
        public bool IsCastle { get; set; }
        public bool IsByCode { get; set; }
        public bool GenerateInFolders { get; set; }
        public bool UseLazy { get; set; }
        public bool IncludeForeignKeys { get; set; }
        public bool NameFkAsForeignTable { get; set; }
        public bool IncludeHasMany { get; set; }
        public bool IncludeLengthAndScale { get; set; }
        public List<string> FieldPrefixRemovalList { get; set; }
        public string TestMethodAttributeName { get; set; }
        public PersistenceTestingFramework PersistenceTestingFramework { get; set; }
        public ServerType ServerType { get; set; }
        public ValidationStyle ValidatorStyle { get; set; }
        public bool GenerateWcfDataContract { get; set; }
        public bool GenerateColumnNameMapping { get; set; }
        public bool BaseClassProvidesIdProperty { get; set; }
    }
}