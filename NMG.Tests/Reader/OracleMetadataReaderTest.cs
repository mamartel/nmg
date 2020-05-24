using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using NMG.Core.Reader;

namespace NMG.Tests.Reader
{
    [TestFixture, Ignore("Requires Oracle client installed.")]
    public class OracleMetadataReaderTest
    {
        private OracleMetadataReader oracleMetadataReader;
        
        [SetUp]
        public void SetUp()
        {
            const string connectionString = "Data Source=xe;User Id=scott;Password=tiger";
            oracleMetadataReader = new OracleMetadataReader(connectionString);
        }

        [Test()]
        public async Task GetOwnersTest()
        {
            var owners = await oracleMetadataReader.GetOwners();
            Assert.IsNotNull(owners);
            Assert.IsTrue(owners.Any());
            Assert.IsTrue(owners.Contains("SCOTT"));
        }
        
        [Test]
        public async Task GetTableTest()
        {
            var tables = await oracleMetadataReader.GetTables("SCOTT");
            Assert.IsNotNull(tables);
            Assert.IsNotEmpty(tables);
            Assert.IsTrue(tables.Any(t => string.Equals(t.Name, "PRODUCTS", StringComparison.OrdinalIgnoreCase)));
        }
        
        [Test]
        public void GetSequencesTest()
        {
            var sequences = oracleMetadataReader.GetSequences("SCOTT");
            Assert.IsNotNull(sequences);
            Assert.IsNotEmpty(sequences);
            Assert.IsTrue(sequences.Any(s => string.Equals(s, "INVENTORY_SEQ", StringComparison.OrdinalIgnoreCase)));
        }
        
        [Test]
        public async Task GetTableDetailsTest()
        {
            var tables = await oracleMetadataReader.GetTables("SCOTT");
            var tableInv = tables.Single(t => string.Equals(t.Name, "INVENTORIES", StringComparison.OrdinalIgnoreCase));
            var invColumns = await oracleMetadataReader.GetTableDetails(tableInv, "SCOTT");
            Assert.IsNotNull(invColumns);
            Assert.IsTrue(invColumns.Any());
            Assert.AreEqual(invColumns.Count, 6);
            Assert.AreEqual(tableInv.PrimaryKey.Type, NMG.Core.Domain.PrimaryKeyType.PrimaryKey);
            Assert.AreEqual(tableInv.ForeignKeys.Count, 2);
            var columnId = invColumns.Single(s => string.Equals(s.Name, "ID"));
            var columnStoreId = invColumns.Single(s => string.Equals(s.Name, "STORE_ID"));
            
            Assert.IsTrue(string.Equals(columnId.DataType, "NUMBER"));
            Assert.IsTrue(string.Equals(columnId.MappedDataType, typeof(Int64)), "Invalid id mapped data type");
            
            Assert.IsTrue(string.Equals(columnStoreId.DataType, "NUMBER"));
            Assert.IsTrue(string.Equals(columnStoreId.MappedDataType, typeof(Int32)), "Invalid store id mapped data type");
        }
    }
}

