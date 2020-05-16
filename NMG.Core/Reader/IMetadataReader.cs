using System.Collections.Generic;
using System.Threading.Tasks;

using NMG.Core.Domain;

namespace NMG.Core.Reader
{
    public interface IMetadataReader
    {
        IList<Column> GetTableDetails(Table table, string owner);
        List<Table> GetTables(string owner);
        Task<IList<string>> GetOwners();
        List<string> GetSequences(string owner);
        PrimaryKey DeterminePrimaryKeys(Table table);
        IList<ForeignKey> DetermineForeignKeyReferences(Table table);
        //List<string> GetSequences(List<Table> table);
        //List<string> GetForeignKeyTables(string columnName);
    }
}