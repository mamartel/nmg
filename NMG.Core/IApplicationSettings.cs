using System;
using System.Collections.Generic;

using NMG.Core.Domain;

namespace NMG.Core
{
    public interface IApplicationSettings
    {
        string NameSpace { get; }
        string NameSpaceMap { get; }
        string AssemblyName { get; }
        Language Language { get; }
        bool IsFluent { get; }
        bool IsEntityFramework { get; }
        string Sequence { get; }
        ClassNamingConvention ClassNamingConvention { get; }
        FieldGenerationConvention FieldGenerationConvention { get; }
        string FolderPath { get; }
        string DomainFolderPath { get; }
        string ForeignEntityCollectionType { get; }
        string InheritenceAndInterfaces { get; }
        string ClassNamePrefix { get; }
        bool PrefixClassName { get; }
        bool EnableInflections { get; }
        bool GeneratePartialClasses { get; }
        FieldNamingConvention FieldNamingConvention { get; }
        string Prefix { get; }
        bool IsCastle { get; }
        bool IsByCode { get; }
        bool GenerateInFolders { get; }
        bool UseLazy { get; }
        bool IncludeForeignKeys { get; }
        bool NameFkAsForeignTable { get; }
        bool IncludeHasMany { get; }
        bool IncludeLengthAndScale { get; }
        List<string> FieldPrefixRemovalList { get; }
        string TestMethodAttributeName { get; }
        PersistenceTestingFramework PersistenceTestingFramework { get; }
        ServerType ServerType { get; }
        ValidationStyle ValidatorStyle { get; }
        bool GenerateWcfDataContract { get; }
        bool GenerateColumnNameMapping { get; }
        bool BaseClassProvidesIdProperty { get; }
        bool QuietNullableWarnings { get; }
    }
}