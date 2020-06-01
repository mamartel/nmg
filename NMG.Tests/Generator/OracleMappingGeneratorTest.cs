using System;
using System.Collections.Generic;

using NMG.Core.Domain;
using NMG.Core.Generator;
using NUnit.Framework;

namespace NMG.Tests.Generator
{
    [TestFixture]
    public class OracleMappingGeneratorTest
    {
        [Test]
        public void ShouldGenerateMappingForOracleTable()
        {
            const string generatedXML = "<?xml version=\"1.0\"?><hibernate-mapping assembly=\"myAssemblyName\" namespace=\"myNameSpace\" xmlns=\"urn:nhibernate-mapping-2.2\"><class name=\"Customer\" table=\"Customer\" lazy=\"true\" xmlns=\"\"><id name=\"Id\" column=\"Id\" /></class></hibernate-mapping>";
            var preferences = new TestApplicationSettings
                                  {
                                      FolderPath = "\\",
                                      AssemblyName = "myAssemblyName",
                                      NameSpace = "myNameSpace",
                                      Sequence = "mySequenceNumber",
                                  };
            var pkColumn = new Column {Name = "Id", IsPrimaryKey = true, DataType = "Int"};
            var primaryKey = new PrimaryKey {Columns = new List<Column> {pkColumn}};
            var generator = new OracleMappingGenerator(preferences, new Table {Name = "Customer", PrimaryKey = primaryKey, Columns = new List<Column> {pkColumn}});
            var document = generator.CreateMappingDocument();
            Assert.AreEqual(generatedXML, document.InnerXml);
        }
    }
}